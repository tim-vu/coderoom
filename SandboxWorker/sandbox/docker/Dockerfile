FROM debian

COPY init.sh .
RUN ./init.sh && rm ./init.sh

COPY install_java.sh .
RUN ./install_java.sh && rm ./install_java.sh

COPY install_dotnet.sh .
RUN ./install_dotnet.sh && rm ./install_dotnet.sh

COPY install_python3.sh .
RUN ./install_python3.sh && rm ./install_python3.sh




