using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Services/FingerprintEnrollmentService.cs
using DPFP;
using DPFP.Capture;
using DPFP.Processing;


namespace DPFP_CaptureExample_ChatGPT.Services
{
    public class FingerprintEnrollmentService : FingerprintService
    {
        private Enrollment _enroller;

        public event Action<Template> TemplateCreated;

        public FingerprintEnrollmentService() : base()
        {
            _enroller = new Enrollment();
        }

        protected override void OnFeaturesExtracted(FeatureSet features)
        {
            if (features != null)
            {
                ReportGenerated?.Invoke("Conjunto de características creado.");
                _enroller.AddFeatures(features);
            }

            UpdateStatus();

            switch (_enroller.TemplateStatus)
            {
                case Enrollment.Status.Ready:
                    TemplateCreated?.Invoke(_enroller.Template);
                    StatusChanged?.Invoke("Plantilla lista. Puede guardar/verificar.");
                    Stop();
                    break;

                case Enrollment.Status.Failed:
                    ReportGenerated?.Invoke("Enrolamiento fallido. Reiniciando...");
                    _enroller.Clear();
                    Stop();
                    Start();
                    break;
            }
        }

        private void UpdateStatus()
        {
            StatusChanged?.Invoke($"Muestras necesarias: {_enroller.FeaturesNeeded}");
        }
    }
}
