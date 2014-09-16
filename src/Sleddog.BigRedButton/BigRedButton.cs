using System.Linq;
using HidLibrary;

namespace Sleddog.BigRedButton
{
    public class BigRedButton
    {
        private static readonly int VendorId = 0x1D34;
        private static readonly int ProductId = 0x000D;
        private IHidDevice device;

        public BigRedButton()
        {
            var hidEnumerator = new HidEnumerator();

            device = hidEnumerator.Enumerate(VendorId, ProductId).FirstOrDefault();
        }
    }
}