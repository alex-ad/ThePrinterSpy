using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class PrinterMaskedNameCollection
    {
        public static ObservableCollection<PrinterMaskedNameNode> PrinterNames { get; }
        private readonly ConfigPrinterNameMask _printerMask;
        private readonly PrintersCollection _printers;

        static PrinterMaskedNameCollection()
        {
            PrinterNames = new ObservableCollection<PrinterMaskedNameNode>();
        }

        public PrinterMaskedNameCollection()
        {
            _printers = new PrintersCollection();
            _printerMask = new ConfigPrinterNameMask();
        }

        public void GetAll()
        {
            var printers = _printers.GetCollection().Where(x=>x.Enabled).ToList();
            if (!printers.Any()) return;

            PrinterNames.Clear();

            while (printers.Any())
            {
                string name = printers[0].Name;
                string mask;
                List<int> ids = new List<int>();
                if (AppConfig.PrinterNameMask.IsEnabled)
                {
                    GetMaskedPrinterName(name, ref printers, out mask, out ids);
                }
                else
                {
                    mask = name;
                    ids.Add(printers[0].Id);
                }
                printers.RemoveAll(x => ids.Contains(x.Id));

                if (string.IsNullOrEmpty(mask)) continue;

                PrinterNames.Add(new PrinterMaskedNameNode
                {
                    Ids = ids,
                    NameMasked = mask
                });
            }
        }

        public void GetMaskedPrinterName(string name, ref List<PrinterNode> printers, out string mask, out List<int> ids)
        {
            string m;
            int len;

            if (_printerMask.Type == ConfigPrinterNameMask.MaskType.BeginName)
            {
                len = Convert.ToInt32(_printerMask.Mask);
                len = len > name.Length ? name.Length : len;
                m = name.Substring(0, len);
                mask = m;
                ids = printers.Where(p => p.Name.StartsWith(m)).Select(i => i.Id).ToList();
            }
            else if (_printerMask.Type == ConfigPrinterNameMask.MaskType.EndName)
            {
                len = Convert.ToInt32(_printerMask.Mask);
                len = len > name.Length ? name.Length : len;
                m = name.Remove(0, name.Length - len);
                mask = m;
                ids = printers.Where(p => p.Name.EndsWith(m)).Select(i => i.Id).ToList();
            }
            else if (_printerMask.Type == ConfigPrinterNameMask.MaskType.WholeName)
            {
                mask = name;
                ids = printers.Where(p => p.Name == name).Select(i => i.Id).ToList();
            }
            else if (_printerMask.Type == ConfigPrinterNameMask.MaskType.ContainsName)
            {
                int start = name.IndexOf(_printerMask.Mask, StringComparison.InvariantCultureIgnoreCase);
                m = start < 0 ? string.Empty : name;
                mask = m;
                ids = printers.Where(p => p.Name.Contains(name)).Select(i => i.Id).ToList();
            }
            else
            {
                Regex regex = new Regex(_printerMask.Mask);
                MatchCollection matches = regex.Matches(name);
                m = (matches.Count < 1) ? name : matches[0].Value;
                mask = (matches.Count < 1) ? string.Empty : matches[0].Value.ToUpper();
                ids = printers.Where(p => p.Name.Contains(m)).Select(i => i.Id).ToList();
            }
        }
    }
}
