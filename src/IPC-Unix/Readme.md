

# IPC via Shared memory (Queue) on UNIX

The demo that uses shared memory on UNIX

# How to build

Build using the following docker command:

```
docker build -t moimhossain/unix-shared-memory-demo .
```

# How to run Producer/Writer

Use the followig command to run the producer

```
docker run --ipc shareable --name producer moimhossain/unix-shared-memory-demo -producer
```

```
 docker rm -v producer
```

> Notice: You need to specify the IPC namespace as **shareable** so Docker engine will map the namespace to the consumer/reader container.

# How to run the Consumer/Reader

Use the following command to run the consumer:

```
 docker run --name consumer --ipc container:producer moimhossain/unix-shared-memory-demo -consumer
```

And for clean up:
```
 docker rm -v consumer
```

# That's it!