FROM docker.io/library/eclipse-temurin:11-jre

ARG ANTLR_VERSION=4.13.1

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /opt/antlr \
    && curl -fsSL "https://www.antlr.org/download/antlr-${ANTLR_VERSION}-complete.jar" \
        -o /opt/antlr/antlr-${ANTLR_VERSION}-complete.jar

ENTRYPOINT ["java", "-jar", "/opt/antlr/antlr-4.13.1-complete.jar"]