FROM trufflesuite/ganache-cli:latest

COPY ganache_with_accounts.sh /app/

RUN /bin/sh -c "apk add --no-cache bash"
CMD ["ganache-cli", "-h", "0.0.0.0"]
ENTRYPOINT bash ganache_with_accounts.sh