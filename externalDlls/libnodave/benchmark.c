extern int seconds, thirds;
    void rBenchmark(daveConnection * dc, int bmArea) {
	int i,res,maxReadLen,areaNumber;
	double usec;
	PDU p;
#ifdef UNIX_STYLE    
	struct timeval t1, t2;
#endif    
#ifdef BCCWIN    
	clock_t t1, t2;
#endif
	seconds=0;thirds=0;
	maxReadLen=dc->maxPDUlength-46;
	areaNumber=0; 
	if(bmArea==daveDB) areaNumber=1;
    	printf("Now going to do read benchmark with minimum block length of 1.\n");
	wait();
#ifdef UNIX_STYLE    
	gettimeofday(&t1, NULL);
#endif    
#ifdef BCCWIN    
	t1=clock();
#endif	    
	for (i=1;i<101;i++) {
    	    daveReadBytes(dc, bmArea, areaNumber,0, 1, NULL);
	    if (i%10==0) {
	        printf("...%d",i);
	        fflush(stdout);
	    }
	}	
#ifdef UNIX_STYLE        
	gettimeofday(&t2, NULL);
	usec = 1e6 * (t2.tv_sec - t1.tv_sec) + t2.tv_usec - t1.tv_usec;
	usec/=1e6;
#endif    
#ifdef BCCWIN    
        t2=clock();
        usec = 0.001*(t2 - t1);
#endif
        printf(" 100 reads took %g secs. \n",usec);
	printf(" retries: 2nd %d 3rd %d\n",seconds, thirds);
	seconds=0;thirds=0;
	
        printf("Now going to do read benchmark with shurely supported block length %d.\n",maxReadLen);
//	daveSetDebug(daveDebugAll);
        wait();
#ifdef UNIX_STYLE    
        gettimeofday(&t1, NULL);
#endif    
#ifdef BCCWIN    
        t1=clock();
#endif	    
        for (i=1;i<101;i++) {
	    daveReadBytes(dc, bmArea, areaNumber, 0, maxReadLen, NULL);
	    if (i%10==0) {
	        printf("...%d",i);
	        fflush(stdout);
	    }
	}	
#ifdef UNIX_STYLE    
	gettimeofday(&t2, NULL);
	usec = 1e6 * (t2.tv_sec - t1.tv_sec) + t2.tv_usec - t1.tv_usec;
	usec/=1e6;
#endif    
#ifdef BCCWIN    
	t2=clock();
	usec = 0.001*(t2 - t1);
#endif
	printf(" 100 reads took %g secs. \n",usec);
	printf(" retries: 2nd %d 3rd %d\n",seconds, thirds);
    	wait();
	seconds=0;thirds=0;
	    
	printf("Now going to do read benchmark with 5 variables in a single request.\n");
	printf("running...\n");
#ifdef UNIX_STYLE    
	gettimeofday(&t1, NULL);
#endif    
#ifdef BCCWIN    
	t1=clock();
#endif	    
	for (i=1;i<101;i++) {
	    davePrepareReadRequest(dc, &p);
	    daveAddVarToReadRequest(&p,daveInputs,0,0,6);
	    daveAddVarToReadRequest(&p,daveFlags,0,0,6);
	    daveAddVarToReadRequest(&p,daveFlags,0,6,6);
	    daveAddVarToReadRequest(&p,bmArea,areaNumber,4,54);
	    daveAddVarToReadRequest(&p,bmArea,areaNumber,4,4);
	    res=daveExecReadRequest(dc, &p, NULL);
	    if (res!=0) printf("\nerror %d=%s\n",res,daveStrerror(res));
	    if (i%10==0) {
	        printf("...%d",i);
	        fflush(stdout);
	    }
	}	
#ifdef UNIX_STYLE    
	gettimeofday(&t2, NULL);
	usec = 1e6 * (t2.tv_sec - t1.tv_sec) + t2.tv_usec - t1.tv_usec;
	usec/=1e6;
#endif    
#ifdef BCCWIN    
	t2=clock();
	usec = 0.001*(t2 - t1);
#endif
	printf(" 100 reads took %g secs.\n",usec);
	printf(" retries: 2nd %d 3rd %d\n",seconds, thirds);
}

void wBenchmark(daveConnection * dc,int bmArea) {	    
    int i, c, areaNumber, maxWriteLen;
    double usec;
#ifdef UNIX_STYLE    
    struct timeval t1, t2;
#endif    
#ifdef BCCWIN    
    clock_t t1, t2;
#endif
    maxWriteLen=dc->maxPDUlength-28;
    areaNumber=0; 
    if(bmArea==daveDB) areaNumber=1;
    printf("Now going to do write benchmark with minimum block length of 1.\n");
    wait();
#ifdef UNIX_STYLE    
    gettimeofday(&t1, NULL);
#endif    
#ifdef BCCWIN    
    t1=clock();
#endif	    
    for (i=1;i<101;i++) {
        daveWriteBytes(dc, bmArea, areaNumber,0,1,&c);
        if (i%10==0) {
	    printf("...%d",i);
	    fflush(stdout);
	}
    }
#ifdef UNIX_STYLE    
    gettimeofday(&t2, NULL);
    usec = 1e6 * (t2.tv_sec - t1.tv_sec) + t2.tv_usec - t1.tv_usec;
    usec/=1e6;
#endif    
#ifdef BCCWIN    
    t2=clock();
    usec = 0.001*(t2 - t1);
#endif
    printf(" 100 writes took %g secs. \n",usec);
	    
    printf("Now going to do write benchmark with shurely supported block length %d.\n",maxWriteLen);
    wait();
#ifdef UNIX_STYLE    
    gettimeofday(&t1, NULL);
#endif    
#ifdef BCCWIN    
    t1=clock();
#endif	    
    for (i=1;i<101;i++) {		
        daveWriteBytes(dc, bmArea, areaNumber, 0, maxWriteLen, &c);
        if (i%10==0) {
	    printf("...%d",i);
	    fflush(stdout);
	}
    }    
#ifdef UNIX_STYLE    
    gettimeofday(&t2, NULL);
    usec = 1e6 * (t2.tv_sec - t1.tv_sec) + t2.tv_usec - t1.tv_usec;
    usec/=1e6;
#endif    
#ifdef BCCWIN    
    t2=clock();
    usec = 0.001*(t2 - t1);
#endif		
    printf(" 100 writes took %g secs. \n",usec);
    wait();
}
/*
    Changes:
    
    03/18/2005	introduced bmArea parameter. S7-200 doesn't have enough Merkers/Flags for long 
		block benchmarks. Must use V memory. This is like DB1 of S7-300/400, but a CPU
		may have a DB1 or not depending on program. So in case of 300/400 use Merker/Flag
		memory.
*/
