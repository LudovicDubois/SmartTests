using System.Collections.Generic;



namespace SmartTestsAnalyzer.Criterias
{
    public class CriteriaValues
    {
        public struct CriteriaValue
        {
            public CriteriaValue( CriteriaAnalysis analysis, bool isError )
            {
                Analysis = analysis;
                IsError = isError;
            }


            public CriteriaAnalysis Analysis { get; }
            public bool IsError { get; }
        }


        public List<CriteriaValue> Values { get; } = new List<CriteriaValue>();


        public void Add( CriteriaAnalysis analysis, bool isError ) => Values.Add( new CriteriaValue( analysis, isError ) );
    }


    public class FieldValues: CriteriaValues
    { }


    public class RangeValues: CriteriaValues
    { }
}