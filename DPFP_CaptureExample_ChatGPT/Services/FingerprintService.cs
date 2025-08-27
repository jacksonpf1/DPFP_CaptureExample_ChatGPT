using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// DPFP
using DPFP;
using DPFP.Processing;
using DPFP.Capture;
using DpfpCapture = DPFP.Capture.Capture;
using DpfpEventHandler = DPFP.Capture.EventHandler;


namespace DPFP_CaptureExample_ChatGPT.Services
{
    public class FingerprintService : DPFP.Capture.EventHandler, IDisposable
    {
        private DpfpCapture _capturer;

        public  Action<string> StatusChanged { get; set; }
        public  Action<string> ReportGenerated { get; set; }
        public  Action<Bitmap> FingerprintCaptured { get; set; }
        public  Action<FeatureSet> FeaturesExtracted { get; set; }

        public FingerprintService()
        {
            try
            {
                _capturer = new DpfpCapture();
                if (_capturer != null)
                    _capturer.EventHandler = this;
                else
                    StatusChanged?.Invoke("No se pudo iniciar el lector de huellas.");
            }
            catch
            {
                StatusChanged?.Invoke("Error al inicializar el capturador.");
            }
        }

        public void Start() => _capturer?.StartCapture();
        public void Stop() => _capturer?.StopCapture();

        public void Dispose() => Stop();

        private Bitmap ConvertSampleToBitmap(Sample sample)
        {
            var convertor = new SampleConversion();
            Bitmap bitmap = null;
            convertor.ConvertToPicture(sample, ref bitmap);
            return bitmap;
        }

        private FeatureSet ExtractFeatures(Sample sample, DPFP.Processing.DataPurpose purpose)
        {
            var extractor = new DPFP.Processing.FeatureExtraction();
            CaptureFeedback feedback = CaptureFeedback.None;
            var features = new FeatureSet();
            extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);

            return feedback == CaptureFeedback.Good ? features : null;
        }

        protected virtual void OnFeaturesExtracted(FeatureSet features)
        {
            // Por defecto no hace nada
        }

        /*public void OnComplete(object capture, string readerSerialNumber, Sample sample)
        {
            ReportGenerated?.Invoke("Huella capturada.");
            StatusChanged?.Invoke("Escanee nuevamente.");
            FingerprintCaptured?.Invoke(ConvertSampleToBitmap(sample));

            var features = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (features != null)
                OnFeaturesExtracted(features);
        }*/

        #region EventHandler Members
        public void OnComplete(object capture, string readerSerialNumber, Sample sample)
        {
            ReportGenerated?.Invoke("Huella capturada.");
            StatusChanged?.Invoke("Escanee nuevamente la huella.");
            FingerprintCaptured?.Invoke(ConvertSampleToBitmap(sample));

            var features = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (features != null)
                FeaturesExtracted?.Invoke(features);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) =>
            ReportGenerated?.Invoke("Dedo retirado.");

        public void OnFingerTouch(object Capture, string ReaderSerialNumber) =>
            ReportGenerated?.Invoke("Sensor tocado.");

        public void OnReaderConnect(object Capture, string ReaderSerialNumber) =>
            ReportGenerated?.Invoke("Lector conectado.");

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) =>
            ReportGenerated?.Invoke("Lector desconectado.");

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback feedback) =>
            ReportGenerated?.Invoke(feedback == CaptureFeedback.Good
                ? "Calidad de muestra: buena."
                : "Calidad de muestra: mala.");
        #endregion
    }
}
