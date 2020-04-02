using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Exceptions;

namespace ThePrinterSpyControl.Models
{
    public class ComputersCollection : IEnumerable
    {
        private static int _id = 0;

        private ObservableCollection<Computer> Computers { get; }

        public ComputersCollection()
        {
            Computers = new ObservableCollection<Computer>();
        }

        public Computer Add(string name)
        {
            if (IsExists(name))
                return GetComputer(name);
            _id++;
            Computer computer = new Computer(_id, name);
            Computers.Add(computer);
            return computer;
        }

        public void Remove(int id)
        {
            Computers.Remove(GetComputer(id));
        }

        public void Remove(string name)
        {
            Computers.Remove(GetComputer(name));
        }

        public void Remove(Computer computer)
        {
            Computers.Remove(computer);
        }

        public void Clear()
        {
            _id = 0;
            Computers.Clear();
        }

        public int Count() => Computers.Count();

        public Computer GetComputer(int id) => Computers.FirstOrDefault(c => c.Id == id);

        public Computer GetComputer(string name) => Computers.FirstOrDefault(c => c.Name == name);

        public string GetComputerName(int id) => Computers.FirstOrDefault(c => c.Id == id)?.Name;

        public string GetComputerName(Computer computer) => Computers.FirstOrDefault(c => c.Id == computer.Id).Name;

        public int GetComputerId(string name) => Computers.FirstOrDefault(c => c.Name == name).Id;

        public int GetComputerId(Computer computer) => Computers.FirstOrDefault(c => c.Id == computer.Id).Id;

        public bool IsExists(Computer computer) => Computers.FirstOrDefault(c => c.Name == computer.Name) != null;

        public bool IsExists(int id) => Computers.FirstOrDefault(c => c.Id == id) != null;

        public bool IsExists(string name) => Computers.FirstOrDefault(c => c.Name == name) != null;

        /*IEnumerator IEnumerable.GetEnumerator()
        {
            return Computers.GetEnumerator();
        }*/

        public IEnumerator GetEnumerator()
        {
            if (Computers != null)
            {
                foreach (Computer c in Computers)
                {
                    yield return c;
                }
            }
        }

        public IEnumerable ConvertToString()
        {
            if (Computers != null)
            {
                foreach (Computer c in Computers)
                {
                    yield return $"Id: {c.Id}; Name: {c.Name}";
                }
            }
        }
    }
}
