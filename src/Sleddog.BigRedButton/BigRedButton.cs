using System;
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
            Observable.Create<BigRedButtonState>(obs =>
            {
                Observable
                    .Interval(TimeSpan.FromMilliseconds(100))
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

            return Observable.Empty<BigRedButtonState>();
        }

        private BigRedButtonState? QueryState()
        {
            var writeResult = device.Write(StatusReport);

            if (!writeResult)
            {
                return null;
            }

            var readData = device.Read(100);

            if (readData.Status != HidDeviceData.ReadStatus.Success)
            {
                return null;
            }

            var statusByte = readData.Data[1];

            var buttonIsPressed = (statusByte & (1 << 0)) == 0;
            var lidIsOpen = (statusByte & (1 << 1)) == 1;

            return new BigRedButtonState(buttonIsPressed, lidIsOpen);
        }
    }
}