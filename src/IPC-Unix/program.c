

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <time.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <errno.h>
#include <mqueue.h>

#define QUEUE_NAME "/sharedqueue"
#define BUFFER_SIZE 1024
#define EXIT_MSG_SIGNAL "done"
#define MODE "-producer"

int main(int argc, char *argv[])
{
    if (argc < 2)
    {
        producer();
    }
    else if (argc >= 2 && 0 == strncmp(argv[1], MODE, strlen(MODE)))
    {
        producer();
    }
    else
    {
        consumer();
    }
}

int producer()
{
    mqd_t mq;
    struct mq_attr attr;
    char buffer[BUFFER_SIZE];
    int msg, i;

    attr.mq_flags = 0;
    attr.mq_maxmsg = 10;
    attr.mq_msgsize = BUFFER_SIZE;
    attr.mq_curmsgs = 0;

    mq = mq_open(QUEUE_NAME, O_CREAT | O_WRONLY, 0644, &attr);
    // seed random 
    srand(time(NULL));
    i = 0;
    while (i < 500) 
    {
        msg = rand() % 256;
        memset(buffer, 0, BUFFER_SIZE);
        sprintf(buffer, "%x", msg);
        printf("Written to Queue: %s\n", buffer);
        fflush(stdout);
        mq_send(mq, buffer, BUFFER_SIZE, 0);
        i=i+1;
    }
    memset(buffer, 0, BUFFER_SIZE);
    sprintf(buffer, EXIT_MSG_SIGNAL);
    mq_send(mq, buffer, BUFFER_SIZE, 0); 

    mq_close(mq);
    mq_unlink(QUEUE_NAME);
    return 0;
}

int consumer()
{
    struct mq_attr attr;
    char buffer[BUFFER_SIZE + 1];
    ssize_t bytes_read;
    mqd_t mq = mq_open(QUEUE_NAME, O_RDONLY);
    if ((mqd_t)-1 == mq) {
        printf("Queue is inaccessible or empty...\n");
        exit(1);
    }
    do {
        bytes_read = mq_receive(mq, buffer, BUFFER_SIZE, NULL);
        buffer[bytes_read] = '\0';
        printf("Read from Queue: %s\n", buffer);
    } while (0 != strncmp(buffer, EXIT_MSG_SIGNAL, strlen(EXIT_MSG_SIGNAL)));

    mq_close(mq);
    return 0;
}