using System.Collections.Generic;
using BindableBase = Prism.Mvvm.BindableBase;

namespace SmartAblationSystem.ViewModels
{
    public enum ColumnSortState
    {
        Ascending, Descending, NotSorted
    }
    public class SortVisualStateManager
    {
        public class ColumnState : BindableBase
        {
            private ColumnSortState state;

            public ColumnState(string columnName, ColumnSortState state = ColumnSortState.NotSorted)
            {
                ColumnName = columnName;
                State = state;
            }
            public string ColumnName { get; }
            public ColumnSortState State { get => state; set => this.SetProperty(ref state, value); }
        }
        public SortVisualStateManager()
        {
            columnsState.Add(nameof(TreatmentRecordsViewModel.ProcedureDate), new ColumnState(nameof(TreatmentRecordsViewModel.ProcedureDate)));
            columnsState.Add(nameof(TreatmentRecordsViewModel.PatientFirstName), new ColumnState(nameof(TreatmentRecordsViewModel.PatientFirstName)));
            columnsState.Add(nameof(TreatmentRecordsViewModel.PatientLastName), new ColumnState(nameof(TreatmentRecordsViewModel.PatientLastName)));
            columnsState.Add(nameof(TreatmentRecordsViewModel.ProcedureID), new ColumnState(nameof(TreatmentRecordsViewModel.ProcedureID)));
            columnsState.Add(nameof(TreatmentRecordsViewModel.Physician), new ColumnState(nameof(TreatmentRecordsViewModel.Physician)));
        }
        private readonly Dictionary<string, ColumnState> columnsState = new Dictionary<string, ColumnState>();

        public ColumnState this[string columnName]
        {
            get => columnsState[columnName];
        }

        public void SortColumn(string columnName, bool isAscending)
        {
            foreach(var key in columnsState.Keys)
            {
                if(key == columnName)
                {
                    columnsState[columnName].State = isAscending ? ColumnSortState.Ascending : ColumnSortState.Descending;
                }
                else columnsState[key].State = ColumnSortState.NotSorted;
            }
        }
    }
}