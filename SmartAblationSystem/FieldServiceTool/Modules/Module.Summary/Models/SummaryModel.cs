using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using Unity;
using static System.DateTime;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.SessionStatus;
using static System.String;

namespace Module.Summary.Models
{
	public class SummaryModel : BindableBase
	{
		public SummaryModel(IUnityContainer container, IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_eventAggregator.GetEvent<RetryRationaleEvent>().Subscribe(OnReceiveRetryRationaleEvent);
			_eventAggregator.GetEvent<SessionStatusEvent>().Subscribe(OnReceiveSessionStatusEvent);
			_eventAggregator.GetEvent<ErrorListUpdateEvent>().Subscribe(OnErrorEvent);
			_eventAggregator.GetEvent<TimeOutEvent>().Subscribe(OnTimeOutEvent);
		}

		private void OnErrorEvent(IList<ErrorMessageExtender> errorLst)
		{
			var timeHeader_ = Now + WhiteSpace + Dash + WhiteSpace;
			foreach(var error_ in errorLst)
			{
				Notes = Concat($"{timeHeader_}{error_.Item4}{Period}{NewLine}", Notes);
			}
		}

		private void OnTimeOutEvent(string timeOutMsg)
		{
			var timeHeader_ = Now + WhiteSpace + Dash + WhiteSpace;
			Notes = Concat($"{timeHeader_}{timeOutMsg}{Period}{NewLine}", Notes);
		}

		private void OnReceiveRetryRationaleEvent((string id, string msg) retryRationaleValueTuple)
		{
			var rationale_ = retryRationaleValueTuple.id.Contains(FlowMeterCheckMessageTitle) 
				? $"{Now}{WhiteSpace}{Dash}{WhiteSpace}{retryRationaleValueTuple.id}{WhiteSpace}{Colon}{WhiteSpace}{retryRationaleValueTuple.msg}{NewLine}"
				: $"{Now}{WhiteSpace}{Dash}{WhiteSpace}{retryRationaleValueTuple.id}{WhiteSpace}{Dash}{WhiteSpace}{RetryRationaleTitle}{Colon}{WhiteSpace}{retryRationaleValueTuple.msg}{NewLine}";
			Notes = Concat(rationale_, Notes);
		}

		private readonly IEventAggregator _eventAggregator;

		private string _Notes = Empty;
		public string Notes
		{
			get => _Notes;
			set
			{
				SetProperty(ref _Notes, value);
				_eventAggregator.GetEvent<SummaryEvent>().Publish(Notes);
			}
		}

		private void OnReceiveSessionStatusEvent((SessionStatus status, DateTime dateTime) sessionStatusEvent)
		{
			var timeHeader_ = sessionStatusEvent.dateTime + WhiteSpace + Dash + WhiteSpace;
			switch(sessionStatusEvent.status)
			{
				case Unknown:
					break;
				case Ready:
					break;
				case Starting:
					Notes = Empty;
					//Notes = Concat($"{timeHeader_}{StartingTestMsg}{Period}{NewLine}", Notes);
					break;
				case Started:
					//Notes = Concat($"{timeHeader_}{StartTestMsg}{Period}{NewLine}", Notes);
					break;
				case Pausing:
					//Notes = Concat($"{timeHeader_}{PausingTestMsg}{Period}{NewLine}", Notes);
					break;
				case Paused:
					//Notes = Concat($"{timeHeader_}{PauseTestMsg}{Period}{NewLine}", Notes);
					break;
				case Resuming:
					//Notes = Concat($"{timeHeader_}{ResumingTestMsg}{NewLine}", Notes);
					break;
				case Resumed:
					//Notes = Concat($"{timeHeader_}{ResumeTestMsg}{NewLine}", Notes);
					break;
				case Stopping:
					//Notes = Concat($"{timeHeader_}{StoppingTestMsg}{NewLine}", Notes);
					break;
				case Stopped:
					//Notes = Concat($"{timeHeader_}{StopTestMsg}{NewLine}", Notes);
					break;
				case Finished:
					break;
				case Finishing:
					break;
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
