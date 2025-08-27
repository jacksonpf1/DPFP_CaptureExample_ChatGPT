using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DPFP;
using DPFP_CaptureExample_ChatGPT.Services;

namespace DPFP_CaptureExample_ChatGPT.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly FingerprintEnrollmentService _service;

        public DelegateCommand StartCommand { get; }
        public DelegateCommand StopCommand { get; }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _report;
        public string Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }

        private BitmapImage _fingerprintImage;
        public BitmapImage FingerprintImage
        {
            get => _fingerprintImage;
            set => SetProperty(ref _fingerprintImage, value);
        }

        private Template _template;
        public Template Template
        {
            get => _template;
            private set => SetProperty(ref _template, value);
        }

        public MainWindowViewModel(FingerprintEnrollmentService service)
        {
            _service = service;
            _service.StatusChanged += s => Status = s;
            _service.ReportGenerated += r => Report += r + Environment.NewLine;
            _service.FingerprintCaptured += img => FingerprintImage = Convert(img);
            _service.TemplateCreated += t => Template = t;

            // comandos
            StartCommand = new DelegateCommand(StartCapture);
            StopCommand = new DelegateCommand(StopCapture);
        }

        private void StartCapture() => _service.Start();
        private void StopCapture() => _service.Stop();

        private BitmapImage Convert(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }
    }
}
