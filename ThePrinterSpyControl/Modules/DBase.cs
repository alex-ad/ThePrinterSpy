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
        private static PrintSpyEntities _context;

        public DBase()
        {
            _context = new PrintSpyEntities();
        }

        public async void SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
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

        public async Task<IEnumerable<User>> GetUsersList() => await _context.Users.ToListAsync();

        public async Task<int> GetUsersCount() => await _context.Users.CountAsync();

        #endregion

        #region Printers

        public async Task<IEnumerable<Printer>> GetPrintersList() => await _context.Printers.ToListAsync();

        public async Task<IEnumerable<Printer>> GetEnabledPrintersList() => await _context.Printers.Where(x => x.Enabled).ToListAsync();

        public async Task<IEnumerable<Printer>> GetPrintersByUser(User user) =>
            await _context.Printers.Where(x => x.UserId == user.Id).ToListAsync();

        public async Task<Printer> GetPrinterById(int id) => await _context.Printers.FirstOrDefaultAsync(x=>x.Id == id);

        public async Task<IEnumerable<Printer>> GetPrintersFromComputer(Computer computer) =>
            await _context.Printers.Where(x => x.ComputerId == computer.Id).ToListAsync();

        public async Task<int> GetPrintersCount() => await _context.Printers.CountAsync();

        public async Task<int> GetEnabledPrintersCount() => await _context.Printers.CountAsync(x => x.Enabled);

        public async Task<int> GetEnabledPrintersByUserCount(User user) =>
            await _context.Printers.CountAsync(x => x.UserId == user.Id && x.Enabled);

        #endregion

        #region Computers

        public async Task<IEnumerable<Computer>> GetComputersList() => await _context.Computers.ToListAsync();

        public async Task<int> GetComputersCount() => await _context.Computers.CountAsync();

        #endregion

        #region PrintData

        public async Task<IEnumerable<PrintData>> GetDataByPrinter(Printer printer) =>
            await _context.PrintDatas.Where(x => x.PrinterId == printer.Id).ToListAsync();

        #endregion
    }
}
