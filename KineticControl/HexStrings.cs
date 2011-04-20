﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KineticControl
{
    class HexStrings
    {
        private const String PS_SEARCH = "0401dc4a01000100000000000a0101de";
        private const String _findOne = "0401dc4a0100090100000000a0310567";
        private const String _findTwo = "0401dc4a0100010000000000a9fe8889";
        private static String _findThree = "0401dc4a01000100000000000a0101de";
        private static String _findFour = "0401dc4a01000a00a9fe319af8acfe03";
        private const String _initalHex =  "0401dc4a01000801000000000000000002ef00000002f0ff";
        private const String _intialHex2 = "0401dc4a010008010000000000000000011b00000002f0ff";
        public const String addressOff = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public const string byteStringOne = "0401dc4a01000801000000000000000001ef00000002f0ff0000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" + 
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000";

        public const string byteStringTwo = "0401dc4a01000801000000000000000002ef00000002f0ff" + "44499900000000FF000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                                            "000000000000000000000000000000000000000000000000000";

        public const string byteStringThree = "0401dc4a010008010000000000000000021b00000002f0ff00000000000000000000000000000000000000000000"+
                                              "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"+
                                              "000000000000000000000000000000000000000000000000000000";


        public static byte[] PsSearch
        {
            get { return DecodeString(PS_SEARCH); }
        }

        public static byte[] DataOne
        {
            get { return DecodeString(_findOne); }
        }

        public static byte[] DataTwo
        {
            get { return DecodeString(_findTwo); }
        }

        public static byte[] DataThree
        {
            get { return DecodeString(_findThree); }
        }

        public static byte[] DataFour
        {
            get { return DecodeString(_findFour); }
        }

        public static byte[] IntialHex
        {
            get { return DecodeString(_initalHex); }
        }

        public static byte[] IntialHex2
        {
            get { return DecodeString(_intialHex2); }
        }

        public static byte[] AddressOff
        {
            get { return DecodeString(addressOff); }
        }

        public static byte[] DecodeString(String hexString)
        {
            byte[] hexBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length / 2; i++)
            {
                hexBytes[i] = Byte.Parse(hexString.Substring(i * 2, 2).ToUpper(), NumberStyles.AllowHexSpecifier);
            }
            return hexBytes;
        }
    }
}
