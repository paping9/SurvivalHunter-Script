using System;
using System.Collections.Generic;
using Defs;
using CalcluationFunc = System.Func<Defs.StatusType, double>;

namespace Game.Status
{
    public class StatusContainer
    {
        private Dictionary<StatusType, LinkedList<StatusValue>> _statusValues;
        private Dictionary<StatusType, double> _calculatedValues;

        public enum CalculationMode
        {
            SumThenApplyRate,       // 모든 Point 합산 후 Rate 적용
            ApplyRateSequentially,  // 순서대로 적용
            Max
        }
        
        private CalculationMode _calcMode;
        private readonly CalcluationFunc[] _calcFunctions;

        public double this[StatusType type] => _calculatedValues[type];

        public StatusContainer(CalculationMode mode = CalculationMode.SumThenApplyRate)
        {
            _statusValues = new Dictionary<StatusType, LinkedList<StatusValue>>();
            _calculatedValues = new Dictionary<StatusType, double>();
            _calcMode = mode;

            _calcFunctions = new CalcluationFunc[(int)CalculationMode.Max];
            
            _calcFunctions[(int)CalculationMode.SumThenApplyRate] = CalcSumThenApplyRate;
            _calcFunctions[(int)CalculationMode.ApplyRateSequentially] = CalcApplyRateSequentially;
        }
        

        public void Add(StatusType type, StatusValue value)
        {
            _statusValues.TryAdd(type, new LinkedList<StatusValue>());
            _statusValues[type].AddLast(value);
            Calc(value.Kind.Type);
        }

        public void Remove(StatusValue value)
        {
            if (_statusValues.TryGetValue(value.Kind.Type, out var list))
            {
                list.Remove(value);
                Calc(value.Kind.Type);
            }
        }

        public void Clear(StatusType type)
        {
            if (_statusValues.ContainsKey(type))
            {
                _statusValues.Remove(type);
                _calculatedValues.Remove(type);
            }
        }
        
        private void Calc(StatusType type)
        {
            _calculatedValues[type] = _calcFunctions[(int)_calcMode](type);
        }
        
        private double CalcSumThenApplyRate(StatusType type)
        {
            if (!_statusValues.TryGetValue(type, out var values) || values.Count == 0)
                return 0.0;

            double pointSum = 0;
            double rateSum = 0;

            foreach (var value in values)
            {
                if (value.Kind.ValueType == StatusValueType.Point)
                    pointSum += value.Value;
                else if (value.Kind.ValueType == StatusValueType.Rate)
                    rateSum += value.Value;
            }
            return pointSum * (1.0 + rateSum / 10000.0);
        }
        
        private double CalcApplyRateSequentially(StatusType type)
        {
            if (!_statusValues.TryGetValue(type, out var values) || values.Count == 0)
                return 0.0;

            double result = 0;
            foreach (var value in values)
            {
                if (value.Kind.ValueType == StatusValueType.Point)
                    result += value.Value;
                else if (value.Kind.ValueType == StatusValueType.Rate)
                    result *= (1.0 + value.Value / 10000.0);
            }
            return result;
        }
    }
}