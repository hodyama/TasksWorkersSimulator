namespace TaskSimulation.Utiles
{
    public class InputXmlShema
    {
        public Version V;

        public Execution[] Executions;

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public class Version
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int V { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public class Execution
        {
            /// <remarks/>
            public ExecutionTaskArrivalRate TaskArrivalRate { get; set; }

            /// <remarks/>
            public ExecutionWorkerArrivalRate WorkerArrivalRate { get; set; }

            /// <remarks/>
            public ExecutionWorkerLeaveRate WorkerLeaveRate { get; set; }

            /// <remarks/>
            public ExecutionWorkersQualityDistribution WorkersQualityDistribution { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string GradeSystem { get; set; }
          

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public long InitialNumOfWorkers { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double MaxSimulationTime { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public long warm_up_time { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public long Seed { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionTaskArrivalRate
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkerArrivalRate
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkerLeaveRate
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkersQualityDistribution
        {
            /// <remarks/>
            public ExecutionWorkersQualityDistributionFeedback Feedback { get; set; }

           

            /// <remarks/>
            public ExecutionWorkersQualityDistributionQuality Quality { get; set; }

          

            /// <remarks/>
            public ExecutionWorkersQualityDistributionResponseTime ProcessingTime { get; set; }

           
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkersQualityDistributionFeedback
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
        }

       

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkersQualityDistributionQuality
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
        }

       

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ExecutionWorkersQualityDistributionResponseTime
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Type { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Params { get; set; }
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public double[] Ratio { get; set; }
        }

        public static readonly InputXmlShema Default = new InputXmlShema()
        {
            V = new Version() { V = 1 },
            Executions = new Execution[]
            {
                new Execution()
                {
                    
                    InitialNumOfWorkers = 3,
                    MaxSimulationTime = 10,
                    warm_up_time=0,
                    Seed = 0,
                    GradeSystem="AQL",
                    TaskArrivalRate = new ExecutionTaskArrivalRate()
                    {
                        Type = "ContinuousUniform",
                        Params = new double[] {1, 2}
                    },
                    WorkerArrivalRate = new ExecutionWorkerArrivalRate()
                    {
                        Type = "ContinuousUniform",
                        Params = new double[] {1, 2}
                    },
                    WorkerLeaveRate = new ExecutionWorkerLeaveRate()
                    {
                        Type = "ContinuousUniform",
                        Params = new double[] {1, 2}
                    },

                    WorkersQualityDistribution = new ExecutionWorkersQualityDistribution()
                    {
                        Feedback = new ExecutionWorkersQualityDistributionFeedback()
                        {
                            Type = "ContinuousUniform",
                            Params = new double[] {1, 2}
                        },
                      
                        Quality = new ExecutionWorkersQualityDistributionQuality()
                        {
                            Type = "ContinuousUniform",
                            Params = new double[] {1, 2}
                        },
                      
                        ProcessingTime = new ExecutionWorkersQualityDistributionResponseTime()
                        {
                            Type = "ContinuousUniform",
                            Params = new double[] {1, 2},
                            Ratio= new double[]{1}
                        },
                       
                    }
                }
            }
        };
    }
}
