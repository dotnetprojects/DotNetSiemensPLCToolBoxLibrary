int simMaxPDUlength=240;
//int simMaxPDUlength=90;
//int simMaxPDUlength=480;
// int simMaxPDUlength=960;

#define runModeStop 0
#define runModeRun 1
int runStop=runModeStop; 


/* Parameter: 00,01,12,08,12,83,01,00,00,00,00,00,*/
/* Data     : FF,09,00,1C,*/

uc CpuTimeStamp[] = {
    0x00,0x19,0x05,0x08,0x23,0x04,0x10,0x23,0x67,0x83,
};

uc blockList[] = {
0x30,0x38,0x00,0x01,
0x30,0x45,0x00,0x00,
0x30,0x43,0x00,0x00,
0x30,0x41,0x00,0x02,
0x30,0x42,0x00,0x09,
0x30,0x44,0x00,0x3B,
0x30,0x46,0x00,0x07,
};


/*
typedef struct _szl {
    us ID;
    us index;
    us elements;
    us elementLength;
}
*/


uc SZL_0_0[] = { 0,2,0,53, //    53 elements of 2 bytes
0x00,0x00,
0x0F,0x00,
0x00,0x11,
0x01,0x11,
0x0F,0x11,
0x00,0x12,
0x01,0x12,
0x0F,0x12,
0x00,0x13,
0x00,0x14,
0x00,0x15,
0x01,0x15,
0x00,0x17,
0x01,0x17,
0x0F,0x17,
0x00,0x18,
0x01,0x18,
0x0F,0x18,
0x00,0x19,
0x0F,0x19,
0x0F,0x1A,
0x0F,0x1B,
0x00,0x1A,
0x00,0x1B,
0x00,0x21,
0x0A,0x21,
0x0F,0x21,
0x02,0x22,
0x00,0x23,
0x0F,0x23,
0x00,0x24,
0x01,0x24,
0x04,0x24,
0x05,0x24,
0x01,0x31,
0x01,0x32,
0x02,0x32,
0x00,0x74,
0x01,0x74,
0x0F,0x74,
0x0C,0x91,
0x0D,0x91,
0x0A,0x91,
0x00,0x92,
0x02,0x92,
0x06,0x92,
0x0F,0x92,
0x00,0xB1,
0x00,0xB2,
0x00,0xB3,
0x00,0xB4,
0x00,0xA0,
0x01,0xA0,
};

uc SZL_25_0[] = { 
0x0,0x19,0,0,		// 25, 0
0,4,0,5, // 5 elements of 4 bytes
0x00,0x01,0x00,0x00,
0x00,0x04,0x01,0x00,
0x00,0x05,0x00,0x00,
0x00,0x06,0x00,0x00,
0x00,0x08,0x01,0x00,
};

uc SZL_273_1[] = { 
0x1,0x11,0,1,		// 273,1
0,28,0,1, // 1 element of 28 bytes
0x00,0x08,'6','E','S','7',' ','3','1','5','-','2','A','F','0','3','-','0','A','B','0',' ',0x00,0xC0,0x00,0x02,0x00,0x00,
};

uc SZL_274_512[] = { 
0x1,0x12,2,0,		// result SZL ID 0112 200 	274, 512 
0,2,0,0, // 0 elements of 2
};


uc SZL_292_0[] = { 
0x1,0x24,0,0,		// result SZL ID 0124 00 
0,20,0,1,		// 1 elements of 20 bytes
0x43,0x02,0xFF,0x68,0xC7,0x00,0x00,0x00,0x00,0x24,0x07,0x24,0x04,0x03,0x21,0x05,0x17,0x42,0x61,0x01,
};


uc SZL_305_1[] = { 
0x1,0x31,0,1,		// 305,1
0,40,0,1, // 1 element of 40 bytes
0x00,0x01,0x00,0xF0,0x00,0x0C,0x00,0x02,0xDC,0x6C,0x00,0x02,0xDC,0x6C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_2[] = {
0x1,0x31,0,2,		// 305,1
0,40,0,1, // 1 element of 40 bytes
0x00,0x02,0xBE,0xFD,0x0F,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3C,0x01,0x00,0x00,0x00,0x00,0x7D,0x00,0x00,0x05,0x03,0x04,0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x0A,0x00,0x00,0x00,0x09,0x00,0x00,
};

uc SZL_305_3[] = { 
0x1,0x31,0,3,		// 305,1
0,40,0,1, // 1 element of 40 bytes
0x00,0x03,0x7F,0xF0,0x83,0x01,0x00,0x20,0x00,0x04,0x00,0x01,0x02,0x09,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_4[] = {
0x1,0x31,0,4,		// 305,4
0,40,0,1, // 1 element of 40 bytes
0x00,0x04,0xFE,0x01,0x22,0x41,0x23,0x00,0x00,0x00,0x10,0x10,0x10,0x04,0x02,0x00,0x00,0x02,0x00,0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_5[] = { 
0x1,0x31,0,5,		// 305,5
0,40,0,1, // 1 element of 40 bytes
0x00,0x05,0x3E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x00,0x64,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_6[] = { 
0x1,0x31,0,6,		// 305,6
0,40,0,1, // 1 element of 40 bytes
0x00,0x06,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0x0E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x4C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_7[] = { 
0x1,0x31,0,7,		// 305,7
0,40,0,1, // 1 element of 40 bytes
0x00,0x07,0x01,0x00,0x3F,0x00,0x08,0x01,0x01,0x00,0x00,0x00,0x01,0x04,0x01,0x04,0x01,0x04,0x20,0x08,0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_8[] = { 
0x1,0x31,0,8,		// 305,8
0,40,0,1, // 1 element of 40 bytes
0x00,0x08,0x70,0x10,0x70,0x06,0x70,0x06,0x41,0xF4,0x41,0xF4,0x41,0xF4,0x41,0xF4,0x43,0x20,0x43,0x20,0x43,0x20,0x43,0x20,0x43,0x20,0x43,0x20,0x43,0x20,0x43,0x20,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_305_9[] = { 
0x1,0x31,0,9,		// 305,9
0,40,0,1, // 1 element of 40 bytes
0x00,0x09,0x04,0x06,0x01,0x00,0x01,0xF7,0x01,0xF7,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_306_1[] = { 
0x1,0x32,0,1,	//result SZL ID 0132 01
0,40,0,1,	//1 elements of 40 bytes
0x00,0x01,0x00,0x01,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0A,0x00,0x00,0x00,0x14,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x00,0x01,
};

uc SZL_306_2[] = { 
0x1,0x32,0,2,	//result SZL ID 0132 02
0,40,0,1,	//1 elements of 40 bytes
0x00,0x02,0x00,0x00,0x00,0x00,0x00,0x05,0x00,0x0E,0x00,0x00,0x00,0x00,0x06,0x01,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
};

uc SZL_306_4[] = { 
0x1,0x32,0,4,	//result SZL ID 0132 04 
0,40,0,1,	//1 elements of 40 bytes
/*
Run:  0x00,0x04,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x01,0x00,0x00,0x00,0x00,0x56,0x56,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
*/
/* RunP: */
 0x00,0x04,0x00,0x01,0x00,0x00,0x00,0x01,0x00,0x02,0x00,0x00,0x00,0x00,0x56,0x56,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
/* Stop: 
0x00,0x04,0x00,0x01,0x00,0x00,0x00,0x01,0x00,0x03,0x00,0x00,0x00,0x00,0x56,0x56,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
*/

};

uc SZL_1060_0[] = { 
0x4,0x24,0,0,	//    result SZL ID 0424 00 
0,20,0,1,	//    1 elements of 20 bytes
0x51,0x42,0xFF,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x03,0x21,0x06,0x31,0x41,0x49,0x61,
};

uc SZL_1060_0S[] = { 
0x4,0x24,0,0,	//    result SZL ID 0424 00 
0,20,0,1,	//    1 elements of 20 bytes
0x51,0x0B,0xFF,0x03,0xC7,0x00,0x00,0x00,0x00,0x10,0x07,0x20,0x04,0x03,0x19,0x21,0x39,0x45,0x29,0x06,
};

uc SZL_1060_0RP[] = { 
0x51,0x42,0xFF,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x03,0x21,0x15,0x27,0x35,0x93,0x71,
};

uc SZL_1316_20480[] = { 
0x5,0x24,0x50,0x0,	//result SZL ID 0524 5000 
0,20,0,1,	//    1 elements of 20 bytes
0x51,0x0B,0xFF,0x03,0xC7,0x00,0x00,0x00,0x00,0x10,0x07,0x20,0x04,0x03,0x19,0x21,0x39,0x45,0x29,0x06,
};

uc SZL_3473_0[] = { 
0xd,0x91,0,0,
0,16,0,1, // SZL-ID 0D91 index 00, 1 element of 16 bytes
0x00,0x00,0x02,0x00,0x7F,0xFF,0x00,0xC0,0x00,0xC0,0x00,0x00,0xB4,0x02,0x00,0x11,
};