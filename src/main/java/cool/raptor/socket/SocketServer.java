package cool.raptor.socket;

import java.io.IOException;

public class SocketServer extends GenericSocket {

    public SocketServer(final int packetSize) {
        super(packetSize);
    }

    @Override
    public void run() {
        long time = System.currentTimeMillis();
        while(true) {
            try {
                if (System.currentTimeMillis() - time > 3000) {
                    receiveData();
                    System.out.println((char) data.getData()[0]);
                    time = System.currentTimeMillis();
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
