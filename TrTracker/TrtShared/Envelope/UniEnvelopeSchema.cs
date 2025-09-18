namespace TrtShared.Envelope
{
    public static class UniEnvelopeSchema
    {
        /// <summary> -> Branch.Name </summary>
        public const string Branch = "Branch";


        /// <summary> -> Testrun.Version </summary>
        public const string Version = "Version";
        /// <summary> -> Testrun.StartedAt : DateTimeOffset? </summary>
        public const string StartedAt = "StartedAt";    // ISO-8601 (UTC)
        /// <summary> -> Testrun.FinishedAt : DateTimeOffset? </summary>
        public const string FinishedAt = "FinishedAt";  // ISO-8601 (UTC)
        /// <summary> -> Testrun.DurationSec : long? </summary>
        public const string DurationMs = "DurationMs";
        /// <summary> -> Testrun.EnvironmentJson : string? </summary>
        public const string EnvironmentJson = "EnvironmentJson";
        /// <summary> -> Testrun.IdempotencyKey : string? </summary>
        public const string IdempotencyKey = "IdempotencyKey";

        /// <summary> -> Testrun in Agregates section </summary>
        public const string Agregates = "Agregates";
        /// <summary> List<TestInfo> </summary>
        public const string Tests = "Tests";
        /// <summary> List<ResultInfo> </summary>
        public const string Results = "Results";

        /// <summary> -> Testrun in Agregates section </summary>
        public static class TestrunAgregates
        {
            /// <summary> -> Testrun.Total : int? </summary>
            public const string Total = "Total";
            /// <summary> -> Testrun.Passed : int? </summary>
            public const string Passed = "Passed";
            /// <summary> -> Testrun.Failed : int? </summary>
            public const string Failed = "Failed";
            /// <summary> -> Testrun.Skipped : int? </summary>
            public const string Skipped = "Skipped";
            /// <summary> -> Testrun.Errors : int? </summary>
            public const string Errors = "Errors";
        }


        /// <summary> -> Test </summary>
        public static class TestInfo
        {
            /// <summary> -> Test.Name </summary>
            public const string Name = "Name";
            /// <summary> -> Test.ClassName : string? </summary>
            public const string ClassName = "ClassName";
            /// <summary> -> Test.Desctiption : string? </summary>
            public const string Desctiption = "Description";
        }


        /// <summary> -> Results </summary>
        public static class ResultInfo
        {
            /// <summary> -> Result.Outcome </summary>
            public const string Outcome = "Outcome";
            /// <summary> -> Result.StartedAt : DateTimeOffset? </summary>
            public const string StartedAt = "StartedAt";    // ISO8601 (UTC)
            /// <summary> -> Result.FinishedAt : DateTimeOffset? </summary>
            public const string FinishedAt = "FinishedAt";  // ISO8601 (UTC)
            /// <summary> -> Result.DurationSec : long? </summary>
            public const string DurationMs = "DurationMs";
            /// <summary> -> Result.ErrType : string? </summary>
            public const string ErrType = "ErrorType";
            /// <summary> -> Result.ErrMsg : string? </summary>
            public const string ErrMsg = "ErrMsg";
            /// <summary> -> Result.ErrStack : string? </summary>
            public const string ErrStack = "ErrStack";
            /// <summary> -> Result.StdOut : string? </summary>
            public const string StdOut = "StdOut";
            /// <summary> -> Result.StdErr : string? </summary>
            public const string StdErr = "StdErr";
        }
    }
}
