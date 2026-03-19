using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YBFramework.Common
{
    public abstract class BaseValue<T> where T : struct
    {
        private static readonly Dictionary<Type, Queue<BaseValue<T>>> s_Pools = new();

        public static TBaseValue Allocate<TBaseValue>() where TBaseValue : BaseValue<T>, new()
        {
            Type type = typeof(TBaseValue);
            if (!s_Pools.TryGetValue(type, out Queue<BaseValue<T>> pool))
            {
                pool = new Queue<BaseValue<T>>();
                s_Pools.Add(type, pool);
            }
            return s_Pools.Count > 0 ? (TBaseValue)pool.Dequeue() : new TBaseValue();
        }

        public static void Free(BaseValue<T> valueInstance)
        {
            Type type = valueInstance.GetType();
            if (!s_Pools.TryGetValue(type, out Queue<BaseValue<T>> pool))
            {
                pool = new Queue<BaseValue<T>>();
                s_Pools.Add(type, pool);
            }
            valueInstance.OnFree();
            pool.Enqueue(valueInstance);
        }

        public static void Free<TBaseValue>(ICollection<TBaseValue> valueInstances) where TBaseValue : BaseValue<T>
        {
            Type valueInstanceType = typeof(TBaseValue);
            if (!s_Pools.TryGetValue(valueInstanceType, out Queue<BaseValue<T>> pool))
            {
                pool = new Queue<BaseValue<T>>();
                s_Pools.Add(valueInstanceType, pool);
            }
            foreach (TBaseValue valueInstance in valueInstances)
            {
                valueInstance.OnFree();
                pool.Enqueue(valueInstance);
            }
        }
        
        protected T m_CurValue;

        private bool m_IsRecordedCurValue;

        private bool m_IsRecordedMaxValue;

        private bool m_IsRecordedMinValue;

        protected T m_MaxValue;

        protected T m_MinValue;

        public void Init(T maxValue, T minValue, T curValue, bool isRecordMaxValue, bool isRecordMinValue, bool isRecordCurValue)
        {
            m_MaxValue = maxValue;
            m_MinValue = minValue;
            m_CurValue = curValue;
            m_IsRecordedMaxValue = isRecordMaxValue;
            m_IsRecordedMinValue = isRecordMinValue;
            m_IsRecordedCurValue = isRecordCurValue;
        }

        private ref bool GetIsEnableRecord(ValueConstraintType valueConstraintType)
        {
            switch (valueConstraintType)
            {
                case ValueConstraintType.Max:
                    return ref m_IsRecordedMaxValue;
                case ValueConstraintType.Min:
                    return ref m_IsRecordedMinValue;
                case ValueConstraintType.Current:
                    return ref m_IsRecordedCurValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Record(ValueConstraintType valueConstraintType, string modifier, T expectedModifiedValue, T actualModifiedValue)
        {
            if (GetIsEnableRecord(valueConstraintType) && !string.IsNullOrEmpty(modifier))
            {
                ValueRecord.AddRecord(this, ValueRecord.Allocate(modifier, ValueConstraintType.Max, expectedModifiedValue, actualModifiedValue));
            }
        }

        public void EnableRecord(ValueConstraintType valueConstraintType, bool enable)
        {
            ref bool isRecorded = ref GetIsEnableRecord(valueConstraintType);
            if (isRecorded != enable)
            {
                if (isRecorded)
                {
                    ValueRecord.RemoveRecordByValueConstraintType(this, valueConstraintType);
                }
                isRecorded = enable;
            }
        }

        public T GetValue(ValueConstraintType valueConstraintType)
        {
            switch (valueConstraintType)
            {
                case ValueConstraintType.Max:
                    return m_MaxValue;
                case ValueConstraintType.Min:
                    return m_MinValue;
                case ValueConstraintType.Current:
                    return m_CurValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetMaxValue()
        {
            return m_MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetMinValue()
        {
            return m_MinValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurValue()
        {
            return m_CurValue;
        }

        public void ModifyValue(ValueConstraintType valueConstraintType, string modifier, T delta)
        {
            switch (valueConstraintType)
            {
                case ValueConstraintType.Max:
                    ModifyMaxValue(modifier, delta);
                    return;
                case ValueConstraintType.Min:
                    ModifyMinValue(modifier, delta);
                    return;
                case ValueConstraintType.Current:
                    ModifyCurValue(modifier, delta);
                    return;
            }
        }

        public abstract void ModifyMaxValue(string modifier, T delta);

        public abstract void ModifyMinValue(string modifier, T delta);

        public abstract void ModifyCurValue(string modifier, T delta);

        protected virtual void OnFree()
        {
            m_MaxValue = default;
            m_MinValue = default;
            m_CurValue = default;
            m_IsRecordedMaxValue = false;
            m_IsRecordedMinValue = false;
            m_IsRecordedCurValue = false;
            ValueRecord.RemoveRecords(this);
        }

        private sealed class ValueRecord
        {
            private static readonly Queue<ValueRecord> s_Pool = new();

            private static readonly Dictionary<BaseValue<T>, List<ValueRecord>> s_ValueRecords = new();

            private static readonly StringBuilder s_StringBuilder = new();

            public static ValueRecord Allocate(string modifier, ValueConstraintType valueConstraintType, T expectedModifiedValue, T actualModifiedValue)
            {
                ValueRecord valueRecord = s_Pool.Count > 0 ? s_Pool.Dequeue() : new ValueRecord();
                valueRecord.Modifier = modifier;
                valueRecord.ValueConstraintType = valueConstraintType;
                valueRecord.ExpectedModifiedValue = expectedModifiedValue;
                valueRecord.ActualModifiedValue = actualModifiedValue;
                return valueRecord;
            }

            private static void Free(ValueRecord valueRecord)
            {
                s_Pool.Enqueue(valueRecord);
            }

            public static void AddRecord(BaseValue<T> valueInstance, ValueRecord record)
            {
                if (!s_ValueRecords.TryGetValue(valueInstance, out List<ValueRecord> valueRecords))
                {
                    //TODO 创建集合存在GC
                    valueRecords = new List<ValueRecord>();
                    s_ValueRecords.Add(valueInstance, valueRecords);
                }

                valueRecords.Add(record);
            }

            public static void RemoveRecordByValueConstraintType(BaseValue<T> valueInstance, ValueConstraintType valueConstraintType)
            {
                if (s_ValueRecords.TryGetValue(valueInstance, out List<ValueRecord> valueRecords))
                {
                    for (int i = 0; i < valueRecords.Count; i++)
                    {
                        if (valueRecords[i].ValueConstraintType == valueConstraintType)
                        {
                            Free(valueRecords[i]);
                            valueRecords.RemoveAt(i);
                        }
                    }
                }
            }

            public static void RemoveRecords(BaseValue<T> valueInstance)
            {
                if (s_ValueRecords.Remove(valueInstance, out List<ValueRecord> valueRecords))
                {
                    for (int i = 0; i < valueRecords.Count; i++)
                    {
                        Free(valueRecords[i]);
                    }
                }
            }

            public static string LogValueRecords(BaseValue<T> valueInstance)
            {
                if (s_ValueRecords.TryGetValue(valueInstance, out List<ValueRecord> valueRecords))
                {
                    s_StringBuilder.Clear();
                    for (int i = 0; i < valueRecords.Count; i++)
                    {
                        AppendValueRecord(valueRecords[i]);
                    }
                    return s_StringBuilder.ToString();
                }
                return null;
            }

            public static string LogValueRecords(BaseValue<T> valueInstance, ValueConstraintType valueConstraintType)
            {
                if (s_ValueRecords.TryGetValue(valueInstance, out List<ValueRecord> valueRecords))
                {
                    s_StringBuilder.Clear();
                    for (int i = 0; i < valueRecords.Count; i++)
                    {
                        if (valueRecords[i].ValueConstraintType == valueConstraintType)
                        {
                            AppendValueRecord(valueRecords[i]);
                        }
                    }
                    return s_StringBuilder.ToString();
                }
                return null;
            }

            //TODO 字符串本地化
            private static void AppendValueRecord(ValueRecord valueRecord)
            {
                s_StringBuilder.Append("Value constraint type:");
                s_StringBuilder.Append(valueRecord.ValueConstraintType);
                s_StringBuilder.Append("Modifier:");
                s_StringBuilder.Append(valueRecord.Modifier);
                s_StringBuilder.Append("Expected modified value:");
                s_StringBuilder.Append(valueRecord.ExpectedModifiedValue);
                s_StringBuilder.Append("Actual modified value");
                s_StringBuilder.Append(valueRecord.ActualModifiedValue);
                s_StringBuilder.Append("\n");
            }
            
            private T ActualModifiedValue;

            private T ExpectedModifiedValue;

            private string Modifier;

            private ValueConstraintType ValueConstraintType;
        }
    }
}