using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Threading;
using Models;

namespace pr4
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MemoryManager memoryManager;
        private ObservableCollection<Models.Task> totalTasklist;
        private ObservableCollection<Models.Task> inProcessTaskList;
        private ObservableCollection<Models.Task> expiredTaskList;
        private ObservableCollection<Models.Task> completedTaskList;


        private int currentTime;

        public ObservableCollection<uint> FreeMemoryAreas => memoryManager.FreeMemoryAreas;

        public ObservableCollection<Models.Task> TotalTaskList
        {
            get { return totalTasklist; }
            set
            {
                totalTasklist = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Models.Task> InProcessTaskList
        {
            get { return inProcessTaskList; }
            set
            {
                inProcessTaskList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Models.Task> ExpiredTaskList
        {
            get { return expiredTaskList; }
            set
            {
                expiredTaskList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Models.Task> CompletedTaskList
        {
            get { return completedTaskList; }
            set
            {
                completedTaskList = value;
                OnPropertyChanged();
            }
        }

        public int CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged(nameof(TimeText));
                OnPropertyChanged();
            }
        }

        public uint TotalMemory => memoryManager.TotalMemory;

        public string TotalMemoryText => $"Total memory: {memoryManager.TotalMemory}";
        public string UsageMemoryText => $"Usage memory: {memoryManager.UsageMemory}";
        public string TimeText => $"Time: {CurrentTime}";

        public ViewModel(uint totalMemory, uint taskCount)
        {
            memoryManager = new MemoryManager(totalMemory);
            totalTasklist = new ObservableCollection<Models.Task>();
            inProcessTaskList = new ObservableCollection<Models.Task>();
            expiredTaskList = new ObservableCollection<Models.Task>();
            completedTaskList = new ObservableCollection<Models.Task>();

            currentTime = 0;

            memoryManager.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MemoryManager.UsageMemory))
                {
                    OnPropertyChanged(nameof(UsageMemoryText));
                }
            };

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTick;
            timer.Start();

            GenerateTasks(taskCount);
        }

        private void OnTick(object? sender, EventArgs e)
        {

            Console.WriteLine(CurrentTime);

            if (TotalTaskList.Count > 0)
            {
                if (TotalTaskList.First().MaxStartTime <= CurrentTime)
                {
                    Models.Task expiredTask = TotalTaskList.First();

                    TotalTaskList.Remove(expiredTask);
                    ExpiredTaskList.Add(expiredTask);
                }
                else if (memoryManager.AllocateMemory(TotalTaskList.First().SizeInBytes) != 0)
                {
                    Models.Task toQueueTask = TotalTaskList.First();

                    TotalTaskList.Remove(toQueueTask);
                    InProcessTaskList.Add(toQueueTask);
                }
            }

            InProcessTaskList.OrderDescending();
            List<Models.Task> toCompleteTask = new List<Models.Task>();
            foreach(var task in InProcessTaskList)
            {
                task.ExecutionDuration--;

                if(task.ExecutionDuration <= 0)
                {
                    toCompleteTask.Add(task);
                }
            }

            foreach (var task in toCompleteTask)
            {
                InProcessTaskList.Remove(task);
                CompletedTaskList.Add(task);
                memoryManager.FreeMemory(task.SizeInBytes);
            }

            CurrentTime++;
        }

        private void GenerateTasks(uint taskCount)
        {
            Random random = new Random();

            List<Models.Task> total = new List<Models.Task>();
            for (uint i = 0; i < taskCount; i++)
            {
                var task = new Models.Task(
                        $"Task{i + 1}",
                        (uint)random.Next(100, 300),
                        (uint)random.Next(10, 50),
                        (uint)random.Next(5, 20)
                    );

                total.Add(task);
            }

          total  =  total.OrderBy(task => task.MaxStartTime).ToList();

            foreach (var task in total)
            {
                TotalTaskList.Add(task);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string? property = null)
        {
            if(property != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
