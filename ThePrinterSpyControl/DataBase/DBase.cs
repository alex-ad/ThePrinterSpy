using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.DataBase
{
    public class DBase : IDisposable
    {
        public class PrintDataCollection
        {
            public PrintData Data;
            public Printer Printer;
            public User User;
        }

        private static PrintSpyEntities _context;

        public DBase()
        {
            _context = new PrintSpyEntities();
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
            catch (CommitFailedException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Users

        public async Task<List<User>> GetUsersList() => await _context.Users.ToListAsync().ConfigureAwait(false);

        #endregion

        #region Printers

        public async Task<List<Printer>> GetPrintersList() => await _context.Printers.ToListAsync().ConfigureAwait(false);

        public async Task<List<Printer>> GetEnabledPrintersList() => await _context.Printers.Where(x => x.Enabled).ToListAsync().ConfigureAwait(false);

        public async Task<List<Printer>> GetPrintersByUser(User user) =>
            await _context.Printers.Where(x => x.UserId == user.Id).ToListAsync().ConfigureAwait(false);

        public async Task<List<Printer>> GetPrintersByUserId(int id) =>
            await _context.Printers.Where(x => x.UserId == id).ToListAsync().ConfigureAwait(false);

        public async Task<Printer> GetPrinterById(int id) => await _context.Printers.FirstOrDefaultAsync(x=>x.Id == id).ConfigureAwait(false);

        public async Task<List<Printer>> GetPrintersByComputer(Computer computer) =>
            await _context.Printers.Where(x => x.ComputerId == computer.Id).ToListAsync().ConfigureAwait(false);

        public async Task<int> GetPrintersCount() => await _context.Printers.CountAsync().ConfigureAwait(false);

        public async Task<int> GetEnabledPrintersCount() => await _context.Printers.CountAsync(x => x.Enabled).ConfigureAwait(false);

        public async Task<int> GetPrintersByUserCount(User user) =>
            await _context.Printers.CountAsync(x => x.UserId == user.Id).ConfigureAwait(false);

        public async Task<int> GetPrintersByComputerCount(Computer computer) =>
            await _context.Printers.CountAsync(x => x.ComputerId == computer.Id).ConfigureAwait(false);

        public async Task<int> GetEnabledPrintersByUserCount(User user) =>
            await _context.Printers.CountAsync(x => x.UserId == user.Id && x.Enabled).ConfigureAwait(false);

        public void SetPrinterEnabled(Printer printer)
        {
            _context.Entry(printer).State = EntityState.Modified;
            SaveChanges();
        }

        public void SetPrinterName(int id, string name)
        {
            var p = GetPrinterById(id).Result;
            if (p == null) return;

            p.Name = name;
            _context.Entry(p).State = EntityState.Modified;
            SaveChanges();
        }

        public void SetPrinterEnabled(int id, bool enabled)
        {
            var p = GetPrinterById(id).Result;
            if (p == null) return;

            p.Enabled = enabled;
            _context.Entry(p).State = EntityState.Modified;
            SaveChanges();
        }

        public void RemovePrinter(int id)
        {
            var p = _context.Printers.FirstOrDefault(x => x.Id == id);
            if (p == null) return;
            _context.Printers.Remove(p);
            RemovePrintDataByPrinter(p.Id);
            SaveChanges();
        }

        #endregion

        #region Computers

        public async Task<List<Computer>> GetComputersList() => await _context.Computers.ToListAsync().ConfigureAwait(false);

        public async Task<int> GetComputersCount() => await _context.Computers.CountAsync().ConfigureAwait(false);

        public async Task<string> GetComputerNameByPrinterId(int id)
        {
            string computerName = string.Empty;

            var p = await _context.Printers.FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return computerName;

            var c = await _context.Computers.FirstOrDefaultAsync(x => x.Id == p.ComputerId);
            if (c != null) computerName = c.Name;
            return computerName;
        }

        #endregion

        #region PrintData

        public async Task<List<PrintData>> GetDataByPrinter(Printer printer) =>
            await _context.PrintDatas.Where(x => x.PrinterId == printer.Id).ToListAsync().ConfigureAwait(false);

        public async Task<List<PrintDataCollection>> GetDataByUserId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                where d.UserId == id
                      && (
                          (
                              (d.TimeStamp.CompareTo(start) > -1 || d.TimeStamp.CompareTo(start) == 0)
                              && (d.TimeStamp.CompareTo(end) < 1 || d.TimeStamp.CompareTo(end) == 0)
                              && isReport
                          )
                          || (!isReport)
                      )
                      && p.Enabled
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByDepartmentName(string name, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                from p in _context.Printers
                from u in _context.Users
                where d.UserId == u.Id
                      && u.Department == name
                      && d.PrinterId == p.Id
                      && (
                          (
                              (d.TimeStamp.CompareTo(start) > -1 || d.TimeStamp.CompareTo(start) == 0)
                              && (d.TimeStamp.CompareTo(end) < 1 || d.TimeStamp.CompareTo(end) == 0)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByComputerId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                where d.ComputerId == id
                      && (
                          (
                              (d.TimeStamp.CompareTo(start) > -1 || d.TimeStamp.CompareTo(start) == 0)
                              && (d.TimeStamp.CompareTo(end) < 1 || d.TimeStamp.CompareTo(end) == 0)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByPrinterId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                where d.PrinterId == id
                      && (
                          (
                              (d.TimeStamp.CompareTo(start) > -1 || d.TimeStamp.CompareTo(start) == 0)
                              && (d.TimeStamp.CompareTo(end) < 1 || d.TimeStamp.CompareTo(end) == 0)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByPrintersGroup(List<int> ids, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                from i in ids
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                where d.PrinterId == i
                      && (
                          (
                              (d.TimeStamp.CompareTo(start) > -1 || d.TimeStamp.CompareTo(start) == 0)
                              && (d.TimeStamp.CompareTo(end) < 1 || d.TimeStamp.CompareTo(end) == 0)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public void RemovePrintDataByPrinter(int id)
        {
            var data = from d in _context.PrintDatas
                where d.PrinterId == id
                select d;
            if (!data.Any()) return;
            _context.PrintDatas.RemoveRange(data);
        }

        #endregion
    }
}
