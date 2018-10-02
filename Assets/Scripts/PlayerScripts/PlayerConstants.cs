namespace Player
{
    public static class Constants
    {
        public const float JUMP_FORCE = 150f;
        public const float JUMP_SPEED = 6f;
        public const float AIR_SPEED_MULTIPLIER = 0.05f;
        public const float JUMP_DECAY = 0.6f;

        public static float MOVE_FORCE = 365f/2;
        public const float MAX_SPEED = 3f;
        public const float GROUND_SPEED_MULTIPLIER = 0.2f;

        public const float SLIDE_INITIAL_MULTIPLIER = 0.6f;
        public const float SLIDE_DRAG = 0.9f;
        public const int SLIDE_TIMER = 2;
        public const float SLIDE_FORCE = 20f;

        public const float GROUND_RADIUS = 0.5f;
        public const int GROUND_CHECK_TIMER = 5;
        

        public const string IDLE = "Idle";
        public const string GROUNDED = "Grounded";
        public const string MOVEMENT = "Movement";
        public const string JUMP = "Jump";
        public const string CROUCH = "Crouch";
        public const string SHOOT = "Shoot";
        public const string SHOOTSTOP = "ShootStop";
        public const string HURT = "Hurt";
        public const string DODGE = "Dodge";

        public const string SHOOT_FAST_MIRROR = "Shoot";
        public const string SHOOT_SLOW_MIRROR = "Shoot2";
        public const string SHOOT_BULLET = "Shoot";

        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string SPEED = "Speed";

        public const string BLOCKING_LAYER = "Blocking";
    }
}
