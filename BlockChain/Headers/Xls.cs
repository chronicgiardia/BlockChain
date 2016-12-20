﻿namespace BlockChain.Headers {

    class Xls : IExtensions {
        public byte[] Header => new byte[]{ 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 };
        public byte[] Footer => new byte[] { 0xfe, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x57, 0x00, 0x6f, 0x00, 0x72, 0x00, 0x6b, 0x00, 0x62, 0x00, 0x6f, 0x00, 0x6f, 0x00, 0x6b, 0x00 };
        public string Extension => "xls";
    }
}