package cool.raptor;

import cool.raptor.socket.SocketClient;
import cool.raptor.socket.SocketServer;
import java.io.IOException;
import java.net.InetSocketAddress;

public class Game {

    public static void main(String[] args) throws IOException {
        InetSocketAddress clientAddress = new InetSocketAddress("localhost",9090);
        InetSocketAddress serverAddress = new InetSocketAddress("localhost",9091);

        SocketServer server = new SocketServer(512);
        server.open(serverAddress);
        server.start();

        SocketClient client = new SocketClient(512);
        client.open(clientAddress);
        client.start();
    }
}
