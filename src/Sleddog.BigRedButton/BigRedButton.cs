using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HidLibrary;

namespace Sleddog.BigRedButton
{
    public class BigRedButton
    {
        private static readonly int VendorId = 0x1D34;
        private static readonly int ProductId = 0x000D;
        private readonly IHidDevice device;

        private static readonly byte[] StatusReport = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02};

        public BigRedButton()
        {
            var hidEnumerator = new HidEnumerator();

            device = hidEnumerator.Enumerate(VendorId, ProductId).FirstOrDefault();
        }

        public IObservable<BigRedButtonState> Listen()
        {
            return Observable.Create<BigRedButtonState>(obs =>
            {
                Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(_ =>
                    {
                        var result = QueryState();

                        if (result == null)
                        {
                            obs.OnError(new Exception("Unable to connect to Big Red Button"));
                        }

                        obs.OnNext(result.GetValueOrDefault());
                    });

                return Disposable.Empty;
            });
        }

        public BigRedButtonState? QueryState()
        {
            var statusQuery = new HidReport(1) {Data = StatusReport};

            var isQueryWritten = device.WriteReport(statusQuery);

            if (!isQueryWritten)
            {
                return null;
            }

            var readReport = device.ReadReport(100);

            var readData = readReport.Data;

            if (readReport.ReadStatus != HidDeviceData.ReadStatus.Success)
            {
                return null;
            }

            var statusByte = readData[0];

            Debug.WriteLine(Convert.ToString(readData[0],2));

            var buttonIsPressed = (statusByte & (1 << 0)) == 0;
            var lidIsOpen = (statusByte & (1 << 1)) != 0;

            return new BigRedButtonState(buttonIsPressed, lidIsOpen);
        }
    }
}