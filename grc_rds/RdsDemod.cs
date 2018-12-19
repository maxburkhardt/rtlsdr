using System;
using System.Collections.Generic;

namespace GrcRdsServer
{
    class RdsDemod
    {
        readonly GroupBuilder _groupBuilder = new GroupBuilder();

        bool _lastDataSample;
        bool _lastRDSSample;
        double _sampleClock;

        public readonly List<byte> _b = new List<byte>();

        // Console.WriteLine(bitcount/ seconds);
        //each sample arrives at a rate of 17500Hz
        // the RDS dataclock is 1187.5Hz
        // 17500/1187.5 = 14.73684210526316

        public void Process17500RateStreamSamples()
        {
            while (_b.Count > 3)
            {
                var c = _b.GetRange(0, 4);
                var f = BitConverter.ToSingle(c.ToArray(), 0);
                _b.RemoveRange(0, 4);

                ProcessSample(f >= 0);
            }
        }

        // a sample arrives at a rate of 17500Hz

        private void ProcessSample(bool thisDataSample)
        {
            // transition?

            // a crude huff 'puff loop

            if ((thisDataSample ^ _lastDataSample))
            {
                if (_sampleClock > 10)
                {
                    _sampleClock -= .1;
                }

                if (_sampleClock < 10)
                {
                    _sampleClock += .1;
                }
            }

            _lastDataSample = thisDataSample;

            _sampleClock += (100 / 14.73684210526316);

            if (_sampleClock >= 100) // completed a 1187.5 Hz clock period
            {
                _sampleClock -= 100;
                _groupBuilder.DoRDSDataBit(DifferentialDecode(thisDataSample));
            }
        }

        // section 1.6 
        // IEC 62106:1999
        // Standard EN50067: 1998,

        private bool DifferentialDecode(bool thisDataSample)
        {
            bool thisConversion = thisDataSample ^ _lastRDSSample;
            _lastRDSSample = thisDataSample;
            return thisConversion;
        }
    }
}
