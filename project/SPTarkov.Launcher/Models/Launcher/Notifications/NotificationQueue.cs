using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace SPTarkov.Launcher.Models.Launcher.Notifications
{
    public class NotificationQueue : INotifyPropertyChanged
    {
        private Timer queueTimer = new Timer();
        public ObservableCollection<NotificationItem> queue { get; set; } = new ObservableCollection<NotificationItem>();

        private bool _ShowBanner;
        public bool ShowBanner
        {
            get => _ShowBanner;
            set
            {
                if(_ShowBanner != value)
                {
                    _ShowBanner = value;
                    RaisePropertyChanged(nameof(ShowBanner));
                }
            }
        }

        public NotificationQueue(int ShowTimeInMiliseconds)
        {
            ShowBanner = false;
            queueTimer.Interval = ShowTimeInMiliseconds;
            queueTimer.Elapsed += QueueTimer_Elapsed;
        }

        public void CloseQueue()
        {
            queue.Clear();
            queueTimer.Stop();
            ShowBanner = false;
        }

        private void CheckAndShowNotifications()
        {
            if(!queueTimer.Enabled)
            {
                ShowBanner = true;
                queueTimer.Start();
            }
        }

        public void Enqueue(string Message, bool AllowNext = false)
        {
            if (queue.Where(x => x.Message == Message).Count() == 0)
            {
                queue.Add(new NotificationItem(Message));

                CheckAndShowNotifications();

                if(AllowNext && queue.Count == 2)
                {
                    Next(true);
                }
            }
        }

        public void Enqueue(string Message, string ButtonText, Action ButtonAction, bool AllowNext = false)
        {
            if (queue.Where(x => x.Message == Message && x.ButtonText == ButtonText && x.ItemAction == ButtonAction).Count() == 0)
            {
                queue.Add(new NotificationItem(Message, ButtonText, ButtonAction));
                CheckAndShowNotifications();

                if (AllowNext && queue.Count == 2)
                {
                    Next(true);
                }
            }
        }

        public void Next(bool ResetTimer = false)
        {
            queue.RemoveAt(0);

            if (queue.Count <= 0)
            {
                CloseQueue();
                return;
            }

            if(ResetTimer)
            {
                queueTimer.Stop();
                queueTimer.Start();
            }
        }

        private void QueueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Next();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
