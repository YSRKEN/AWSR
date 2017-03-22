/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     CompatilizedRandom.cs
 * Class:
 *     Rei.Random.CompatilizedRandom
 * Purpose:
 *     A compatilizer of Rei.Random.RandomBase with System.Random.
 * Remark:
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// Rei.Random.RandomBase��System.Random�݊��ɂ���A�_�v�^�N���X�B
    /// �e�����o�֐��̓��́E�o�͂�System.Random�Ɠ����͈͂ɂȂ�܂��B
    /// Rei.Random.RandomBase�h���N���X��System.Random�Ƃ��Ďg�������ꍇ�ɗp���܂��B
    /// </summary>
    public class CompatilizedRandom : System.Random {
        private RandomBase original;

        /// <summary>
        /// rand���\�[�X�Ƃ��ăA�_�v�^�N���X�����������܂��B
        /// </summary>
        public CompatilizedRandom( RandomBase rand ) {
            original = rand;
        }

        /// <summary>
        /// [0,Int32.MaxValue)�̋[���������擾���܂��B
        /// </summary>
        public override Int32 Next() {
            uint r;
            do
                r = original.NextUInt32() & 0x7FFFFFFF;
            while (r == Int32.MaxValue);
            return (Int32)r;
        }

        /// <summary>
        /// [0,maxValue)�̋[���������擾���܂��B
        /// �A��maxValue=0�̏ꍇ��0��Ԃ��܂��B
        /// </summary>
        public override Int32 Next( Int32 maxValue ) {
            return Next(0, maxValue);
        }

        /// <summary>
        /// [minValue,maxValue)�̋[���������擾���܂��B
        /// �������AminValue=maxValue�̂Ƃ���minValue��Ԃ��܂��B
        /// </summary>
        public override Int32 Next( Int32 minValue, Int32 maxValue ) {
            if (minValue > maxValue) throw new ArgumentOutOfRangeException();
            if (minValue == maxValue) return minValue;
            UInt32 range = (UInt32)((Int64)maxValue - minValue);
            UInt32 residue = (UInt32.MaxValue - range + 1) % range;// (MaxValue+1) % range �ł��ƃI�[�o�[�t���[����̂�range�������Ă����B
            UInt32 r;
            do {
                r = original.NextUInt32();
            } while (r < residue);
            return (Int32)((Int64)((r - residue) % range) + minValue);
        }

        /// <summary>
        /// [0,1)�̋[���������擾���܂��B
        /// </summary>
        protected override double Sample() {
            return this.Next() * 4.6566128752457969E-10;
        }

        /// <summary>
        /// �o�C�g�z��ɋ[���������i�[���܂��B
        /// </summary>
        public override void NextBytes( byte[] buffer ) {
            original.NextBytes(buffer);
        }

    }

}