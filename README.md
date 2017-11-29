# MrChopper - Online CS2D

Special assignment for Advanced Techniques of videogames course at [ITBA](https://www.itba.edu.ar) 

## Usage

This is a Unity game, to try it please download this repository and import it to Unity. 

### Commands 

| Command   |    Functionality |
|:----------:|:-------------:
| SpaceBar | Start the simulation |
| LeftArrow | Move left |
| RightArrow | Move right |
| DownArrow | Move down |
| UpArrow | Move up |
| K | Stops interpolation |

### Configuration 

You can alter this configurations on the `client` 

| Parameter  |  Example |
|:----------:|:------:|
| serverPort | 9000 |  
| clientPort | 9001 | 
| playerId | 1 |
| buffDesiredLength | 5 |  
| maxDiffTime | 1 |  
| simSpeed | 1 |  
| frameRate | 1/60 |  

You can alter this configurations on the `server` 

| Parameter  |  Example |
|:----------:|:------:|
| serverPort | 9000 |  
| clientPort | 9001 |  
| fakeDelay | 0.3 |  
| fakePacketLoss | 15 |  
| snapshotSendRate | 60 |  


## Credits

* Forked from [y0rshl](https://github.com/y0rshl/TAVJ).
* [Fraga, Matias](https://github.com/matifraga)

</br>