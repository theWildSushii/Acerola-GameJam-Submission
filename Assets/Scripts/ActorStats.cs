[System.Serializable]
public class ActorStats {

    public float baseHP = 10f;
    public float currentHP = 10f;
    public float moveSpeed = 5f;
    public float moveAcceleration = 16f;
    public float aimSpeed = 180f;
    public float baseArmor = 1f;

    public float NormalizedHP {
        get {
            return currentHP / baseHP;
        }
        protected set {
            currentHP = value * baseHP;
        }
    }
}