namespace OpenGLWinForms.Shaders;

internal class FragmentShader : IShader
{
    public string SourceCode => @"
        #version 330

        uniform vec3 uColor;

        in  vec3  vN;		// normal vector
        in  vec3  vL;		// vector from point to light
        in  vec3  vE;		// vector from point to eye

        void main(void)
        {
            float uKa = 0.33;
            float uKd = 0.33;
            float uKs = 0.33;
            float uShininess = 20.;
            vec3 uSpecularColor = vec3(0.7, 0.7, 0.6);

            vec3 Normal	= normalize(vN);
            vec3 Light	= normalize(vL);
            vec3 Eye	= normalize(vE);

            // Adding per fragment lighting code
            vec3 ambient = uKa * uColor;

            float d = max( dot(Normal,Light), 0. );	// only do diffuse if the light can see the point
            vec3 diffuse = uKd * d * uColor;

            float s = 0.;
            if( dot(Normal,Light) > 0. )			// only do specular if the light can see the point
            {
                vec3 ref = normalize(  reflect( -Light, Normal )  );
            s = pow( max( dot(Eye,ref),0. ), uShininess );
            }
            vec3 specular = uKs * s * uSpecularColor;
            gl_FragColor = vec4( ambient + diffuse + specular,  1. );

            //gl_FragColor = vec4(uColor, 1.0);
        }
    ";
}
