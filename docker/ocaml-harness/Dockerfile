FROM ocaml/opam:alpine-ocaml-4.14-fp

WORKDIR /workspace

RUN opam update && opam install -y dune

ENTRYPOINT ["sh"]