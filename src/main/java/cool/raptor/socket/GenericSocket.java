package cool.raptor.socket;

import cool.raptor.interfaces.SimpleSocket;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketAddress;
import java.net.SocketException;

public class GenericSocket extends Thread implements SimpleSocket {

    /* default */ DatagramSocket socket;
    /* default */ DatagramPacket data;

    public GenericSocket(final int packetSize) {
        data = new DatagramPacket(new byte[packetSize], packetSize);
    }

    @Override
    public void sendData(final DatagramPacket data) throws IOException {
        socket.send(data);
    }

    @Override
    public void receiveData() throws IOException {
        socket.receive(data);
    }

    @Override
    public void open(final int port) throws SocketException {
        if (!isOpen()) {
            socket = new DatagramSocket(port);
        }
    }

    @Override
    public void open(final SocketAddress address) throws SocketException {
        if (!isOpen()) {
            socket = new DatagramSocket(address);
        }
    }

    @Override
    public void close() {
        if (isOpen()) {
            socket.close();
        }
    }

    @Override
    public boolean isOpen() {
        return socket != null && !socket.isClosed();
    }
}
