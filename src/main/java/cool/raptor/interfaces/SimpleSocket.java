package cool.raptor.interfaces;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.SocketAddress;
import java.net.SocketException;

public interface SimpleSocket {

    public void sendData(final DatagramPacket data) throws IOException;
    public void receiveData() throws IOException;
    public void open(final int port) throws SocketException;
    public void open(final SocketAddress address) throws SocketException;
    public void close();
    public boolean isOpen();
}
