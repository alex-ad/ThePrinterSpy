using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.Modules
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
                _context.SaveChanges();
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

        public List<User> GetUsersList() => _context.Users.ToList();

        public int GetUsersCount() => _context.Users.Count();

        #endregion

        #region Printers

        public List<Printer> GetPrintersList() => _context.Printers.ToList();

        public List<Printer> GetEnabledPrintersList() => _context.Printers.Where(x => x.Enabled).ToList();

        public List<Printer> GetPrintersByUser(User user) =>
            _context.Printers.Where(x => x.UserId == user.Id).ToList();

        public List<Printer> GetPrintersByUserId(int id) =>
            _context.Printers.Where(x => x.UserId == id).ToList();

        public Printer GetPrinterById(int id) => _context.Printers.FirstOrDefault(x=>x.Id == id);

        public List<Printer> GetPrintersByComputer(Computer computer) =>
            _context.Printers.Where(x => x.ComputerId == computer.Id).ToList();

        public int GetPrintersCount() => _context.Printers.Count();

        public int GetEnabledPrintersCount() => _context.Printers.Count(x => x.Enabled);

        public int GetPrintersByUserCount(User user) =>
            _context.Printers.Count(x => x.UserId == user.Id);

        public int GetPrintersByComputerCount(Computer computer) =>
            _context.Printers.Count(x => x.ComputerId == computer.Id);

        public int GetEnabledPrintersByUserCount(User user) =>
            _context.Printers.Count(x => x.UserId == user.Id && x.Enabled);

        public void SetPrinterEnabled(Printer printer)
        {
            _context.Entry(printer).State = EntityState.Modified;
            SaveChanges();
        }

        public void SetPrinterName(int id, string name)
        {
            var p = GetPrinterById(id);
            if (p == null) return;

            p.Name = name;
            _context.Entry(p).State = EntityState.Modified;
            SaveChanges();
        }

        public void SetPrinterEnabled(int id, bool enabled)
        {
            var p = GetPrinterById(id);
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

        public List<Computer> GetComputersList() => _context.Computers.ToList();

        public int GetComputersCount() => _context.Computers.Count();

        public string GetComputerNameByPrinterId(int id)
        {
            string computerName = string.Empty;

            var p = _context.Printers.FirstOrDefault(x => x.Id == id);
            if (p == null) return computerName;

            var c = _context.Computers.FirstOrDefault(x => x.Id == p.ComputerId);
            if (c != null) computerName = c.Name;
            return computerName;
        }

        #endregion

        #region PrintData

        public List<PrintData> GetDataByPrinter(Printer printer) =>
            _context.PrintDatas.Where(x => x.PrinterId == printer.Id).ToList();

        public List<PrintDataCollection> GetDataByUserId(int id, DateTime start, DateTime end, bool isReport)
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
                select new PrintDataCollection { Data = d, Printer = p, User = u };

            return data.ToList();
        }

        public List<PrintDataCollection> GetDataByDepartmentName(string name, DateTime start, DateTime end, bool isReport)
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

            return data.ToList();
        }

        public List<PrintDataCollection> GetDataByComputerId(int id, DateTime start, DateTime end, bool isReport)
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

            return data.ToList();
        }

        public List<PrintDataCollection> GetDataByPrinterId(int id, DateTime start, DateTime end, bool isReport)
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
            
            var ret = data.ToList();

            return ret;
        }

        public List<PrintDataCollection> GetDataByPrintersGroup(List<int> ids, DateTime start, DateTime end, bool isReport)
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

            return data.ToList();
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
