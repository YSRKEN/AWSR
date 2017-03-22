/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     RandomBase.cs
 * Class:
 *     Rei.Random.RandomBase
 * Purpose:
 *     A base class for random number generator.
 * Remark:
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// �e��[�������W�F�l���[�^�[�p���N���X�B
    /// �h���N���X��NextUInt32����������K�v������܂��B
    /// </summary>
    public abstract class RandomBase {

        /// <summary>
        /// �h���N���X�ŕ����Ȃ�32bit�̋[�������𐶐�����K�v������܂��B
        /// </summary>
        public abstract UInt32 NextUInt32();

        /// <summary>
        /// ��������32bit�̋[���������擾���܂��B
        /// </summary>
        public virtual Int32 NextInt32() {
            return (Int32)NextUInt32();
        }

        /// <summary>
        /// �����Ȃ�64bit�̋[���������擾���܂��B
        /// </summary>
        public virtual UInt64 NextUInt64() {
            return ((UInt64)NextUInt32() << 32) | NextUInt32();
        }

        /// <summary>
        /// ��������64bit�̋[���������擾���܂��B
        /// </summary>
        public virtual Int64 NextInt64() {
            return ((Int64)NextUInt32() << 32) | NextUInt32();
        }

        /// <summary>
        /// �[��������𐶐����A�o�C�g�z��ɏ��Ɋi�[���܂��B
        /// </summary>
        public virtual void NextBytes( byte[] buffer ) {
            int i = 0;
            UInt32 r;
            while (i + 4 <= buffer.Length) {
                r = NextUInt32();
                buffer[i++] = (byte)r;
                buffer[i++] = (byte)(r >> 8);
                buffer[i++] = (byte)(r >> 16);
                buffer[i++] = (byte)(r >> 24);
            }
            if (i >= buffer.Length) return;
            r = NextUInt32();
            buffer[i++] = (byte)r;
            if (i >= buffer.Length) return;
            buffer[i++] = (byte)(r >> 8);
            if (i >= buffer.Length) return;
            buffer[i++] = (byte)(r >> 16);
        }

        /// <summary>
        /// [0,1)�̋[���������擾���܂��B
        /// [0,1)��2^53�ɋϓ��ɂ킯�A���̂������Ԃ��܂��B
        /// NextUInt32��2��Ăяo���܂��B
        /// </summary>
        public virtual double NextDouble() {
            UInt32 r1, r2;
            r1 = NextUInt32();
            r2 = NextUInt32();
            return (r1 * (double)(2 << 11) + r2) / (double)(2 << 53);
        }

    }

}