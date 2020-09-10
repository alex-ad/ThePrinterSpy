using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            public Computer Computer;
        }

        private static PrintSpyEntities _context;

        public DBase()
        {
            try
            {
                _context = new PrintSpyEntities();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        private void SaveChanges()
        {
            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (CommitFailedException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region Users

        public async Task<List<User>> GetUsersList() => await _context.Users.ToListAsync().ConfigureAwait(false);

        public void UpdateUser(UserNode user)
        {
            var u = _context.Users.FirstOrDefault(x => x.Sid == user.Sid);
            if (u == null) return;
            u.Department = user.Department;
            u.AccountName = user.AccountName;
            u.FullName = user.FullName;
            _context.Entry(u).State = EntityState.Modified;
            SaveChanges();
        }

        #endregion

        #region Printers

        public async Task<List<Printer>> GetPrintersList() => await _context.Printers.ToListAsync().ConfigureAwait(false);

        public async Task<Printer> GetPrinterById(int id) => await _context.Printers.FirstOrDefaultAsync(x=>x.Id == id).ConfigureAwait(false);

        public async Task SetPrinterEnabled(int id, bool enabled)
        {
            var p = await GetPrinterById(id);
            if (p == null) return;

            p.Enabled = enabled;
            _context.Entry(p).State = EntityState.Modified;
            SaveChanges();
        }

        public async Task RemovePrinter(int id)
        {
            var p = await _context.Printers.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (p == null) return;
            _context.Printers.Remove(p);
            RemovePrintDataByPrinter(p.Id);
            SaveChanges();
        }

        #endregion

        #region Computers

        public async Task<List<Computer>> GetComputersList() => await _context.Computers.ToListAsync().ConfigureAwait(false);

        #endregion

        #region PrintData

        public async Task<List<PrintDataCollection>> GetDataByUserId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                join c in _context.Computers on d.ComputerId equals c.Id
                where d.UserId == id
                      && (
                          (
                              (DbFunctions.TruncateTime(d.TimeStamp) >= start)
                              && (DbFunctions.TruncateTime(d.TimeStamp) <= end)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u, Computer = c};

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByDepartmentName(string name, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                from p in _context.Printers
                from u in _context.Users
                from c in _context.Computers
                where d.UserId == u.Id
                      && u.Department == name
                      && d.PrinterId == p.Id
                      && d.ComputerId == c.Id
                      && (
                          (
                              (DbFunctions.TruncateTime(d.TimeStamp) >= start)
                              && (DbFunctions.TruncateTime(d.TimeStamp) <= end)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u, Computer = c};

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByComputerId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                join c in _context.Computers on d.ComputerId equals c.Id
                where d.ComputerId == id
                      && (
                          (
                              (DbFunctions.TruncateTime(d.TimeStamp) >= start)
                              && (DbFunctions.TruncateTime(d.TimeStamp) <= end)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u, Computer = c};

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByPrinterId(int id, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                join c in _context.Computers on d.ComputerId equals c.Id
                where d.PrinterId == id
                      && (
                          (
                              (DbFunctions.TruncateTime(d.TimeStamp) >= start)
                              && (DbFunctions.TruncateTime(d.TimeStamp) <= end)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u, Computer = c};

            return await data.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PrintDataCollection>> GetDataByPrintersGroup(List<int> ids, DateTime start, DateTime end, bool isReport)
        {
            var data =
                from d in _context.PrintDatas
                from i in ids
                join p in _context.Printers on d.PrinterId equals p.Id
                join u in _context.Users on d.UserId equals u.Id
                join c in _context.Computers on d.ComputerId equals c.Id
                where d.PrinterId == i
                      && (
                          (
                              (DbFunctions.TruncateTime(d.TimeStamp) >= start)
                              && (DbFunctions.TruncateTime(d.TimeStamp) <= end)
                              && isReport
                          )
                          || (!isReport)
                      )
                select new PrintDataCollection { Data = d, Printer = p, User = u, Computer = c};

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

        #region Servers

        public async Task<List<Server>> GetServersList() => await _context.Servers.ToListAsync().ConfigureAwait(false);

        #endregion
    }
}
