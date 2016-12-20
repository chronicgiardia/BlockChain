﻿using System;
using System.Security.Cryptography;
using BlockChain.Enums;

namespace BlockChain.Extensions {

    internal static class ByteArrayExtensions{

        public static uint ToUInt32(this byte[] data, ref int index){
            var result = BitConverter.ToUInt32(data, index);
            index += 4;
            return result;
        }

        public static int ToInt32(this byte[] data, int index) {
            return BitConverter.ToInt32(data, index);
        }

        public static uint ToUInt32(this byte[] data, int index) {
            return BitConverter.ToUInt32(data, index);
        }

        public static ulong ToUInt64(this byte[] data, ref int index){
            var result = BitConverter.ToUInt64(data, index);
            index += 8;
            return result;
        }

        public static long ToInt64(this byte[] data, ref int index){
            var result = BitConverter.ToInt64(data, index);
            index += 8;
            return result;
        }

        public static OutPoint ToOutPoint(this byte[] data, ref int index){
            return new OutPoint(data, ref index);
        }

        public static BitcoinValue ToBitcoin(this byte[] data, ref int index){
            return new BitcoinValue((decimal) data.ToInt64(ref index)/BitcoinValue.SatoshisPerBitcoin);
        }

        public static SignatureScript ToSignatureScript(this byte[] data, ref int index){
            var scriptLength = data.ToVariableInt(ref index);
            return new SignatureScript(data, ref index, (int) scriptLength);
        }

        public static PublicKeyScript ToPublicKeyScript(this byte[] data, ref int index){
            var scriptLength = data.ToVariableInt(ref index);
            return new PublicKeyScript(data, ref index, (int) scriptLength);
        }


        public static ulong ToVariableInt(this byte[] data, ref int index){

            var value = (ulong) data[index++];

            switch (value){
                case 0xFD:
                    value = BitConverter.ToUInt16(data, index);
                    index += 2;
                    break;

                case 0xFE:
                    value = BitConverter.ToUInt32(data, index);
                    index += 4;
                    break;

                case 0xFF:
                    value = BitConverter.ToUInt64(data, index);
                    index += 8;
                    break;
            }

            return value;
        }

        public static DateTime ToDateTime(this byte[] data, ref int index){
            var value = ToUInt32(data, ref index);
            return DateTime.SpecifyKind(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(value), DateTimeKind.Utc);
        }

        public static int Search(this byte[] haystack, byte[] needle) {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++) {
                var k = 0;
                for (; k < len; k++) {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        public static SHA256 Sha256 = SHA256.Create();

        public static Hash ToSha256_2(this byte[] data, ref int index, int length){
            var hash = Sha256.ComputeHash(Sha256.ComputeHash(data, index, length));

            return new Hash(hash);
        }

        public static Hash ToHash(this byte[] data, ref int index, int length){
            return new Hash(data, ref index, length);
        }

        // OP_HASH160 f80ffc734c5723bf5caa012a0835fa4f8d5e5155 OP_EQUAL 

        public static bool IsOpHashEqual(this byte[] array) {
            return array[0] == Op.OP_HASH160 
                && array[1] == 0x14 
                && array[array.Length - 1] == 0x87;
        }

        // OP_DUP OP_HASH160 27a1f12771de5cc3b73941664b2537c15316be43 OP_EQUALVERIFY OP_CHECKSIG 

        internal static bool IsOpHashEqualCheckSig(this byte[] array) {
            return array[0] == 0xae
                && array[1] == 0x5c
                && array[array.Length - 2] == 0x16
                && array[array.Length - 1] == 0x04;
        }

        // OP_DUP OP_HASH160 ac42e0f55f4f26cb4d350add34ae8f15aa95382b OP_EQUALVERIFY OP_CHECKSIG

        public static bool IsOpDupCheckSig(this byte[] array){
            return array[0] == Op.OP_DUP 
                && array[1] == Op.OP_HASH160 
                && array[2] == 0x14 
                && array[array.Length - 2] == 0x88 
                && array[array.Length - 1] == Op.OP_CHECKSIG;
        }

        public static bool IsOp1(this byte[] array) {
            return array[0] == Op.OP_1 
                && array[1] == Op.OP_SEPARATOR 
                && array[array.Length - 2] == Op.OP_1 
                && array[array.Length - 1] == Op.OP_CHECKMULTISIG;
        }

        public static bool IsOp1V2(this byte[] array) {
            return array[0] == Op.OP_1
                && array[1] == 0x21
                && array[array.Length - 2] == Op.OP_1
                && array[array.Length - 1] == Op.OP_CHECKMULTISIG;
        }

        // OP_1 41a6736b31faa19d88f30f8ebfe8cc8a24236c02bd7151038816c814a1b706f30bcb91028432f69a2ffcb83553666946555b4d5f5cb316444e51499ce8c77e287b fd45efd28550ed5aa782687be240387c85148609cc3dc556d88a023d3d24ce2d8d048c0d0db99b4360cb1565d24706df207573d5a1000000000000000000000000 OP_2 OP_CHECKMULTISIG

        public static bool IsOp2(this byte[] array) {
            return array[0] == Op.OP_1
                && array[1] == Op.OP_SEPARATOR
                && array[array.Length - 2] == Op.OP_2
                && array[array.Length - 1] == Op.OP_CHECKMULTISIG
                && array[67] == Op.OP_SEPARATOR;
        }

        // OP_1 1c2200007353455857696b696c65616b73204361626c6567617465204261636b75700a0a6361626c65676174652d3230313031323034313831312e377a0a0a446f 776e6c6f61642074686520666f6c6c6f77696e67207472616e73616374696f6e732077697468205361746f736869204e616b616d6f746f277320646f776e6c6f61 6420746f6f6c2077686963680a63616e20626520666f756e6420696e207472616e73616374696f6e20366335336364393837313139656637393764356164636364 OP_3 OP_CHECKMULTISIG

        internal static bool IsOp3(this byte[] array){
            return array[0] == Op.OP_1 
                && array[1] == Op.OP_SEPARATOR
                && array[array.Length - 2] == Op.OP_3 
                && array[array.Length - 1] == Op.OP_CHECKMULTISIG
                && array[67] == Op.OP_SEPARATOR
                && array[133] == Op.OP_SEPARATOR;
        }

        // Not sure why, but a record came with a 0x21 instead of a 0x41 as it's second separator.  Maybe that it was a variable length?

        internal static bool IsOp3V2(this byte[] array) {
            return array[0] == Op.OP_1
                && array[1] == Op.OP_SEPARATOR
                && array[array.Length - 2] == Op.OP_3
                && array[array.Length - 1] == Op.OP_CHECKMULTISIG
                && array[67] == Op.OP_SEPARATOR
                && array[133] == 0x21;
        }


    }
}