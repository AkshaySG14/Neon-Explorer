namespace Player
{
    public static class Constants
    {
        public static float MOVE_FORCE = 365f;
        public const float MAX_SPEED = 3f;
        public const float JUMP_SPEED = 6f;
        public const float GROUND_SPEED_MULTIPLIER = 0.2f;
        public const float AIR_SPEED_MULTIPLIER = 0.05f;
        public const float JUMP_FORCE = 150f;
        public const float GROUND_RADIUS = 0.5f;
        public const float JUMP_DECAY = 0.6f;

        public const float SHOOT_FORCE = 200f;

        public const string IDLE = "Idle";
        public const string GROUNDED = "Grounded";
        public const string MOVEMENT = "Movement";
        public const string JUMP = "Jump";
        public const string CROUCH = "Crouch";
        public const string SHOOT = "Shoot";
        public const string SHOOTSTOP = "ShootStop";
        public const string HURT = "Hurt";

        public const string HORIZONTAL = "Horizontal";
        public const string SPEED = "Speed";

        public const string BLOCKING_LAYER = "Blocking";
    }
}
