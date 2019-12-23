﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaAIDetector_SouthStar.View;
using Aqrose.Framework.Utility.MessageManager;
using Aqrose.Framework.Utility.Tools;

namespace AntennaAIDetector_SouthStar.Result
{
    // Singleton
    public class ResultDevice
    {
        private Task.Task _taskDevice = null;
        private static ResultDevice _instance = null;
        private Queue<SingleResult> _singleResults = new Queue<SingleResult>();

        public static Object PAD_LOCK = new object();
        public int TaskSize
        {
            get
            {
                if (null == _taskDevice)
                {
                    return 0;
                }
                else
                {
                    return _taskDevice.TaskSize;
                }
            }
        }

        public int TotalSize
        {
            get
            {
                if (null == _taskDevice)
                {
                    return 0;
                }
                else
                {
                    return _taskDevice.TotalSize;
                }
            }
        }

        private ResultDevice()
        {
            _taskDevice = Task.TaskPool.GetInstance();
        }

        public static ResultDevice GetInstance()
        {
            if (null == _instance)
            {
                lock (PAD_LOCK)
                {
                    if (null == _instance)
                    {
                        _instance = new ResultDevice();
                    }
                }
            }

            return _instance;
        }

        public void Clear()
        {
            _singleResults = new Queue<SingleResult>();

            return;
        }

        public void Enqueue(SingleResult singleResult)
        {
            if (null == singleResult)
            {
                MessageManager.Instance().Warn("Result.Enqueue: null singleResult.");

                return;
            }

            _singleResults.Enqueue(singleResult);

            return;
        }

        public string GenerateHeaderString()
        {
            string res = "";
            int groupCount = TotalSize / TaskSize + Convert.ToInt32(0 != TotalSize / TaskSize);
            int index = 0;

            for (int i = 0; i < groupCount; ++i)
            {
                for (int j = 0; j < TaskSize; ++j)
                {
                    index = i * TaskSize + j;
                    if (TotalSize <= index)
                    {
                        break;
                    }
                    res += (TotalSize.ToString() + "_" + index.ToString() + ",");
                }
            }

            return res;
        }

        public string GenerateDstMessage()
        {
            string res = "";
            int groupCount = TotalSize / TaskSize + Convert.ToInt32(0 != TotalSize / TaskSize);
            /*
             * first dim: channel
             * second dim: index
             */
            string[][] resultArrayOfSingleChannel = new string[5][]
            {
                new string[10]
                {
                    "x", "x", "x", "x", "x",
                    "x", "x", "x", "x", "x"
                },
                new string[10]
                {
                    "x", "x", "x", "x", "x",
                    "x", "x", "x", "x", "x"
                },
                new string[10]
                {
                    "x", "x", "x", "x", "x",
                    "x", "x", "x", "x", "x"
                },
                new string[10]
                {
                    "x", "x", "x", "x", "x",
                    "x", "x", "x", "x", "x"
                },
                new string[10]
                {
                    "x", "x", "x", "x", "x",
                    "x", "x", "x", "x", "x"
                }
            };

            // fill in
            for (int indexOfResultQueue = 0; indexOfResultQueue < TotalSize; ++indexOfResultQueue)
            {
                var singleResult = _singleResults.Dequeue();
                int channel = singleResult.Index;
                int index = indexOfResultQueue / TaskSize;
                resultArrayOfSingleChannel[channel][index]= singleResult.DefectInfo;
            }
            // pick out
            for (int i = 0; i < groupCount; ++i)
            {
                for (int j = 0; j < TaskSize; ++j)
                {
                    var index = i * TaskSize + j;
                    if (TotalSize <= index)
                    {
                        break;
                    }
                    res += (resultArrayOfSingleChannel[j][i] + ",");
                }
            }

            return res;
        }

    }
}
