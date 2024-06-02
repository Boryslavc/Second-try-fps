// Exists solely because object pooler checks type equality to provide proper pool of objects
// So, this class exist to provide new type for object pooler, so it creates new pool of sounds
// and soounds for rpg, ar ans submachine don't get mixed
public class RPGSoundController : SoundController
{

}
