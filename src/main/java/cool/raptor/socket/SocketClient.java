package cool.raptor.socket;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetSocketAddress;

public class SocketClient extends GenericSocket {

    public SocketClient(final int packetSize) {
        super(packetSize);
    }

    @Override
    public void run() {
        long time = System.currentTimeMillis();
        final InetSocketAddress serverAddress = new InetSocketAddress("localhost",9091);
        final DatagramPacket dataToSend = new DatagramPacket("W".getBytes(), "W".getBytes().length, serverAddress);

        while(true) {
            if(System.currentTimeMillis() - time > 3000) {
                try {
                    sendData(dataToSend);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                time = System.currentTimeMillis();
            }
        }
    }
}
