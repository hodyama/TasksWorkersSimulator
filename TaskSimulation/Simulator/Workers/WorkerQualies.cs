using static TaskSimulation.Utiles.InputXmlShema;

namespace TaskSimulation.Simulator.Workers
{
    public class WorkerQualies
    {
        public string FeedbackType { get;  }

        public double[] FeedbackParams { get;  }
  
        public string QualityType { get;  }

        public double[] QualityParams { get;  }

        public string ResponseType { get; }

     
        public double[] ResponseParams { get; }
      

      


        public WorkerQualies(string  feedbackType, double[] feedbackParams,  string qualityType, double[] qualityParams, string responseType, double[] responseParams)
        {
            FeedbackType = feedbackType;
            FeedbackParams = feedbackParams;
            
            QualityType = qualityType;
            QualityParams = qualityParams;
            
            ResponseType = responseType;
            ResponseParams = responseParams;
            
        }

        
    }
}