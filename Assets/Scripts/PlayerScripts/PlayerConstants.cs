namespace Player
{
    public static class Constants
    {
        public const float JUMP_FORCE = 150f*2;
        public const float JUMP_SPEED = 6f;
        public const float AIR_SPEED_MULTIPLIER = 0.05f;
        public const float JUMP_DECAY = 0.6f;

        public static float MOVE_FORCE = 365f/4;
        public const float MAX_SPEED = 3f;
        public const float GROUND_SPEED_MULTIPLIER = 0.2f;

        public const float SLIDE_INITIAL_MULTIPLIER = 0.6f; //Initial horizontal velocity multiplier
        public const float SLIDE_DRAG = 0.9f; //Horizontal slide force is multiplied with this value every frame 
        public const int SLIDE_TIMER = 2; //How long (seconds) the player is gonna slide for
        public const float SLIDE_FORCE = 2000f; //Sliding force

        public const float DJUMP_SIDE_VER_VEL = 4f; //Vertical velocity is set to this value when side djump is activated
        public const float DJUMP_SIDE_VEL_MULTIPLIER = 1.5f; //Side Vel is multiplied with this value when side djump is activated
        public const float DJUMP_SIDE_MIN_VEL = 0.9f; //Minimum side djump vel
        public const float DJUMP_UP_VEL= 6f; //Upwards new velocity when up djump is activated
        public const float DJUMP_UP_SIDE_VEL_MULTIPLIER = 0.5f; //Side velocity is multiplied with this value when up djump is activated
        public const float DJUMP_DOWN_VEL = -4f; //New vertical vel for down djump
        public const float DJUMP_GRAVITY_MULTIPLIER = 1.5f; //New gravity after djump

        public const int CROUCH_DRAG = 10;

        public const float GROUND_RADIUS = 0.5f;
        public const int GROUND_CHECK_TIMER = 5; //For how many frames is the ground collision disabled after landing
        public const int GROUND_CHECK_TIMER_FALL = 15;
        

        public const string IDLE = "Idle";
        public const string GROUNDED = "Grounded";
        public const string MOVEMENT = "Movement";
        public const string JUMP = "Jump";
        public const string CROUCH = "Crouch";
        public const string SHOOT = "Shoot";
        public const string SHOOTSTOP = "ShootStop";
        public const string HURT = "Hurt";
        public const string DODGE = "Dodge";
        public const string AIR = "IsAir";

        public const string SHOOT_FAST_MIRROR = "Shoot";
        public const string SHOOT_SLOW_MIRROR = "Shoot2";
        public const string SHOOT_BULLET = "Shoot";

        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string SPEED = "Speed";

        public const string BLOCKING_LAYER = "Blocking";
    }
}
