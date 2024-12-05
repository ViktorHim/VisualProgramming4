using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Task : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private uint _executionDuration;

        public string Name { get; set; }
        public uint SizeInBytes { get; set; }
        public uint MaxStartTime { get; set; }
    
        public uint ExecutionDuration
        {
            get => _executionDuration;
            set
            {
                _executionDuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProccesInfo));
            }
        }

        public string Info => $"{Name}\t{MaxStartTime}s\t{SizeInBytes}b";
        public string CompleteInfo => $"{Name}\t{SizeInBytes}b";
        public string ProccesInfo => $"{Name}\t{ExecutionDuration}s\t{SizeInBytes}b";


        public Task(string name, uint size, uint startTime, uint duration)
        {
            Name = name;
            SizeInBytes = size;
            ExecutionDuration = duration;
            MaxStartTime = startTime;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MemoryManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<uint> FreeMemoryAreas { get; set; }
        public uint TotalMemory {  get; set; }

        private uint usageMemory;
        public uint UsageMemory
        {
            get => usageMemory;
            set
            {
                if (usageMemory != value)
                {
                    usageMemory = value;
                    OnPropertyChanged();
                }
            }
        }

        public MemoryManager(uint totalMemory)
        {
            FreeMemoryAreas = new ObservableCollection<uint> {totalMemory };
            TotalMemory = totalMemory;
            UsageMemory = 0;
        }

        public uint AllocateMemory(uint size)
        {
            uint maxArea = FreeMemoryAreas.Max();

            if( maxArea >= size )
            {
                uint remainingSize = maxArea - size;
                FreeMemoryAreas.Remove(maxArea);
                UsageMemory += size;

                if (remainingSize > 0)
                {
                    FreeMemoryAreas.Add(remainingSize);
                }

                return size;
            }
                return 0;
        }

        public void FreeMemory(uint size)
        {
            FreeMemoryAreas.Add(size);
            UsageMemory -= size;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
