
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "nodave.h"
#include "openSocket.h"

#include <time.h> // TWI

//#include <sstream>
//#include <string>

const char *pistart_parameter[] = {
    "P01",
    "_N_MPF_DIR/_N_TEILEPROG2_MPF",
    ""
};


void wait() {
    uc c;
//    printf("Press return to continue.\n");
#ifdef UNIX_STYLE
//    read(0,&c,1);
#endif
}    

void usage()
{
    printf("Usage: testISO_TCP [-d] [-w] IP-Address of CP\n");
    printf("-2 uses a protocol variant for the CP243. You need to set it, if you use such a CP.\n");
    printf("-w will try to write to Flag words. It will overwrite FB0 to FB15 (MB0 to MB15) !\n");
    printf("-d will produce a lot of debug messages.\n");
    printf("-b will run benchmarks. Specify -b and -w to run write benchmarks.\n");
    printf("-m will run a test for multiple variable reads.\n");
    printf("-c will write 0 to the PLC memory used in write tests.\n");
    printf("-z will read some SZL list items (diagnostic information).\n"
	    "  Works 300 and 400 family only.\n");
    printf("-s stops the PLC.\n");
    printf("-t reads time from PLC clock.\n");
    printf("-r tries to put the PLC in run mode.\n");
    printf("--readout read program and data blocks from PLC.\n");
    printf("--route=subnetId,subnetId,PLC address. Try routing. ");
    printf("	subnetID are the two values you see in Step7 or NetPro. PLC address is a number (MPI,Profibus) or an IP adress.\n");
    printf("	Examples: --route=0x0125,0x0013,1 connects to PLC 1 in an MPI/Profibus subnet.\n");
    printf("	Examples: --route=0x0125,0x0013,192.168.1.51 connects to PLC with IP 192.168.1.51 in an Ethernet subnet.\n");
    printf("--sync sets the PLC's clock to PC time.\n");
    printf("--readoutall read all program and data blocks from PLC. Includes SFBs and SFCs.\n");
    printf("--slot=<number> sets slot for PLC (default is 2).\n");
    printf("--ram2rom tries to Copy RAM to ROM.\n");    
    printf("--debug=<number> will set daveDebug to number.\n");
    printf("Example: testISO_TCP -w 192.168.19.1\n");
}


#include "benchmark.c"

int main(int argc, char **argv) {
	
	double this_targ = 0.0;
	double targ_zero = -400;
	int digits = 3;
	int r_inter = 20;
	double rand_el = 2;
	int i2 = 0;
	int test = 0;
	
	if(test)
	{
		srand(1);
		for(i2 = 1; i2 < 20; i2++)
		{
			printf("Loop i2: %02d\n", i2);
			this_targ = targ_zero + ((double)i2 * r_inter); 
						
			//this_targ+=round(((double)(rand() - 16382.0)/16382.0) * rand_el,digits);
			
			int iRand = rand();
			printf("iRand: %d\n", iRand);
			
			double iDummy = ((double)(iRand - 16382)/16382);
			printf("iDummy as f: %f\n", iDummy);
			printf("ungerunded: %f\n", ((double)(iRand - 16382)/16382) * rand_el);
			printf("test: %f\n", ((double)(iRand - 16382)/16382) * rand_el);
			
			//this_targ=round(((double)(rand() - 16382.0)/16382.0) * rand_el,digits);
			this_targ += ((((double)(iRand - 16382)/16382) * rand_el)*1000)/1000.0;
			printf("this_targ: %f\n", this_targ);
			printf("\n");
		}
		return;
	}
	
	
	
	
    time_t t;
    struct tm *ts;  /* #include <time.h> ergänzen */
    
    int i,a,b,c,adrPos,doWrite,doBenchmark, doSZLread, doMultiple, doClear,
	res, useProtocol,doSZLreadAll, doRun, doStop, doCopyRAMtoROM, doReadout, doSFBandSFC,
	doSync, doReadTime,
	doTestMany,
	doNewfunctions,
	aLongDB,
	saveDebug, doRouting, doList, doListall,
	useSlot;
#ifdef PLAY_WITH_KEEPALIVE    	
    int opt;
#endif    
    float d;
    daveInterface * di;
    daveConnection * dc;
    _daveOSserialType fds;
    PDU p;
    daveResultSet rs;
    
    char routeargs[100];
    char * first,*second;
    int subnet1;
    int subnet3;
    int PLCadrsize;
    uc PLCaddress[4];
    uc buffer[1000000];
    int length;

    
    daveSetDebug(daveDebugPrintErrors);
    adrPos=1;
    doWrite=0;
    doBenchmark=0;
    doMultiple=0;
    doClear=0;
    doSZLread=0;
    doSZLreadAll=0;
    doRun=0;
    doStop=0;
    doCopyRAMtoROM=0;
    doReadout=0;
    doSFBandSFC=0;
    doSync=0;
    doReadTime=0;
    doNewfunctions=0;
    doRouting=0;
    doList=0;
    doListall=0;
    useProtocol=daveProtoISOTCP;
    useSlot=2;
    

    if (argc<2) {
	usage();
	exit(-1);
    }    
    
    while (argv[adrPos][0]=='-') {
        if (strcmp(argv[adrPos],"-2")==0) {
            useProtocol=daveProtoISOTCP243;
        } else	if (strncmp(argv[adrPos],"--debug=",8)==0) {
            daveSetDebug(atol(argv[adrPos]+8));
            printf("setting debug to: 0x%lx\n",atol(argv[adrPos]+8));
        } else if (strcmp(argv[adrPos],"-s")==0) {
            doStop=1;
        } else if (strcmp(argv[adrPos],"-t")==0) {
            doReadTime=1;
        } else if (strcmp(argv[adrPos],"-r")==0) {
            doRun=1;
        } else if (strncmp(argv[adrPos],"--many=",7)==0) {
            aLongDB=atol(argv[adrPos]+7);
            doTestMany=1;
        } else if (strncmp(argv[adrPos],"--listall",9)==0) {
            doListall=1;
            doList=1;
        } else if (strncmp(argv[adrPos],"--list",6)==0) {
            doList=1;
        } else if (strncmp(argv[adrPos],"--ram2rom",9)==0) {
            doCopyRAMtoROM=1;    
        } else if (strncmp(argv[adrPos],"--readoutall",12)==0) {
            doReadout=1;
            doSFBandSFC=1;
        } else if (strncmp(argv[adrPos],"--readout",9)==0) {
            doReadout=1;
        } else if (strncmp(argv[adrPos],"--route=",8)==0) {
            doRouting=1;
            strncpy(routeargs,argv[adrPos]+8, 100);
            printf("routing arguments: %s\n",routeargs);
            subnet1=strtol(routeargs,&first,16);
            printf("1st part subnet ID: %d\n",subnet1);
            first++;
            subnet3=strtol(first,&first,16);
            printf("2nd part subnet ID: %d\n",subnet3);
            first++;
            printf("rest: %s\n",first);
            PLCaddress[0]=strtol(first,&first,10);
            if (strlen(first)!=0) {
            printf("PLC address is IP\n");
            PLCadrsize=4;
            first++;
            PLCaddress[1]=strtol(first,&first,10);
            first++;
            PLCaddress[2]=strtol(first,&first,10);
            first++;
            PLCaddress[3]=strtol(first,&first,10);
            
            } else {
            printf("PLC address: %d\n", PLCaddress[0]);
            PLCadrsize=1;
            }
        } else if (strncmp(argv[adrPos],"--slot=",7)==0) {
            useSlot=atol(argv[adrPos]+7);
        } else if (strcmp(argv[adrPos],"-d")==0) {
            daveSetDebug(daveDebugAll);
        } else if (strcmp(argv[adrPos],"-n")==0) {
            doNewfunctions=1;
        } else
        if (strcmp(argv[adrPos],"-w")==0) {
            doWrite=1;
        } else
        if (strcmp(argv[adrPos],"-b")==0) {
            doBenchmark=1;
        } else
        if (strcmp(argv[adrPos],"-z")==0) {
            doSZLread=1;
        } else
        if (strcmp(argv[adrPos],"-a")==0) {
            doSZLreadAll=1;
        } else
        if (strcmp(argv[adrPos],"-m")==0) {
            doMultiple=1;
        } else
        if (strncmp(argv[adrPos],"--sync",6)==0) {
            doSync=1;
        } 
        adrPos++;
        if (argc<=adrPos) {
            usage();
            exit(-1);
        }	
    }    
    
    fds.rfd=openSocket(102, argv[adrPos]);
    fds.wfd=fds.rfd;
    
    if (fds.rfd>0) { 
        di =daveNewInterface(fds,"IF1",0, useProtocol, daveSpeed187k);
        daveSetTimeout(di,5000000);
        dc =daveNewConnection(di,2,0,useSlot);  // insert your rack and slot here

        /*if (doRouting) {
            daveSetRoutingDestination(dc, subnet1, subnet3, PLCadrsize, PLCaddress);
        }*/
	
        if (0==daveConnectPLC(dc)) {
            printf("Connected.\n");
            
            printf("Starte GetNC Program...\n");
            
            /* Das NC-Programm */
            /*
			int iPos = 0;
			for (i=1; i<90001; i++) {
				sprintf(buffer+iPos, "G0 F%05d\r\n",i);
				iPos+=strlen("G0 F00000\r\n");
				//std::stringstream ss;
				//ss << "G0 F" << i << "\r\n";
				//std::string s = ss.str();
				//strcpy(buffer, "G0 Fxxxx\r\n");
			};
			sprintf(buffer+iPos, "M30\r\n\r\n");
         */
			//strcpy(buffer, "M30\r\n\r\n");
            //strcpy(buffer, "G4F5\r\nUNTERPROG1\r\nG4F5\r\nUNTERPROG1\r\nG4F5\r\nM30\r\n\r\n");
            //res = davePutNCProgram2(dc, "_N_TEST_DOWNLOAD1_MPF", "/_N_WKS_DIR/_N_TEST_JE_WPD", "160513072915", buffer, strlen(buffer));
            
            uc* param[10];
			int length;
			printf("daveSetDebug: 0x1ffff\n");
			daveSetDebug(0x1ffff);
			//daveSetDebug(daveGetDebug()|daveDebugPDU|daveDebugUpload); //daveDebug & daveDebugUpload

            //############# Upload DIR #########
            printf("Init PI-Service...\n");            
            param[0]= "P01";
            param[1]= "/_N_wks_dir";
            printf("Start PI-Service \"DIR\"...\n");
			res = davePIstart_nc(dc, "_N_F_XFER", &param, 2);
            printf("davePIstart_nc res=%d\n", res);

            //Upload DIR von NC
            printf("Starte daveGetNcFile...\n");
            res = daveGetNcFile(dc, "_N_wks_dir", buffer, &length);
            //######################


            //############ Upload Folder ##########

            //PI-Service
            //PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });
            //davePIstart_nc(daveConnection *dc, const char *piservice, const char *param[], int paramCount)
            printf("Init PI-Service...\n");
            
            param[0]= "P01";
            //param[1]= "/_N_WKS_DIR/_N_TEST_JE_WPD/_N_TEST_UPLOAD2_MPF";
			param[1]= "/_N_wks_dir/_N_G_GQ_TEST_SWITCH_VCS_WPD";
            printf("Start PI-Service...\n");

			//int DECL2 davePIstart_nc(daveConnection *dc, const char *piservice, const char *param[], int paramCount)
			//PI_Service("_N_F_XFER", new string[] { "P01", "/_N_wks_dir/_N_G_GQ_TEST_SWITCH_VCS_WPD" });
            res = davePIstart_nc(dc, "_N_F_XFER", &param, 2);
            printf("davePIstart_nc res=%d\n", res);
            

            //Upload von NC
            printf("Starte daveGetNcFile...\n");
            res = daveGetNcFile(dc, "_N_G_GQ_TEST_SWITCH_VCS_WPD", buffer, &length);
            //daveGetNCfile(dc, "N_TEST_UPLOAD2_MPF", buffer, &length);

            printf("daveGetNcFile res=%d\n", res);
            //######################

            wait();

            closeSocket(fds.rfd);
            printf("Finished.\n");
            
            return 0;
        } else {
            printf("Couldn't connect to PLC.\n Please make sure you use the -2 option with a CP243 but not with CPs 343 or 443.\n");	
            closeSocket(fds.rfd);
            return -2;
        }
    } else {
        printf("Couldn't open TCP port. \nPlease make sure a CP is connected and the IP address is ok. \n");	
    	return -1;
    }    
}
