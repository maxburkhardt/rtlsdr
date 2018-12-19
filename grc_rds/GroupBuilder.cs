namespace GrcRdsServer
{
    class GroupBuilder
    {
        ushort _frame;
        ushort _groupA;
        ushort _groupB;
        ushort _groupC;
        int _groupBitIndex;

        public void DoRDSDataBit(bool thisConversion)
        {
            _frame = (ushort)(_frame << 1);

            if (thisConversion)
                _frame++;

            _groupBitIndex++;

            // a really crude RDS groups synchroniser - forget error correcting syndrome fields - 
            // just look for the PI code in every information word in block 1

            // Heart FM PI code flips during news items so we look for both
            if (((_frame & 0x0ffff) == 0xc663) || ((_frame & 0x0ffff) == 0xc363))
            {
                _groupA = _frame;
                _groupBitIndex = 16;
            }

            switch (_groupBitIndex)
            {
                case 42: //block 2 information word
                    _groupB = _frame;
                    break;

                case 68: //block 3 information word
                    _groupC = _frame;
                    break;

                case 94: //block 4 information word
                    GroupHandler.AnalyseFrames(_groupA, _groupB, _groupC, _frame);
                    break;
            }
        }
    }
}
