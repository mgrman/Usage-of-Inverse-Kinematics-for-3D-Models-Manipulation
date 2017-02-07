using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class SingleParticleControler : MonoBehaviour
{
    private float _oldSize;
    public float Size;

    private Color _oldColor;
    public Color Color;

    private Material _oldMaterial;
    public Material Material;



    ParticleSystem _particleSys;
    ParticleSystem ParticleSys
    {
        get
        {
            if (_particleSys == null)
                _particleSys = this.GetComponent<ParticleSystem>();
            return _particleSys;
        }
    }


	void Start () {

        _oldColor = Color;
        _oldSize = Size;
        _oldMaterial = Material;
        ResetAndEmitOneParticle();
	}

    void Update()
    {
        if (Color != _oldColor || _oldSize != Size || _oldMaterial!=Material)
        {
            _oldColor = Color;
            _oldSize = Size;
            ResetAndEmitOneParticle();
        }

        if (Color != _oldColor || _oldSize != Size || _oldMaterial != Material)
        {
            _oldMaterial = Material;

            var renderer=ParticleSys.GetComponent<ParticleSystemRenderer>();
            renderer.material = Material;
            renderer.sharedMaterial = Material;
        }

        if (ParticleSys.maxParticles != 1)
            ParticleSys.maxParticles = 1;
    }

    void OnEnable()
    {
        ResetAndEmitOneParticle();
    }

    private void ResetAndEmitOneParticle()
    {
        //ParticleSys.Clear();
        ParticleSys.enableEmission = false;
        ParticleSys.startSpeed = 0;

        if (ParticleSys.particleCount == 0)
        {
            ParticleSys.startColor = Color;
            ParticleSys.startSize = Size;

            ParticleSys.startLifetime = float.PositiveInfinity;
            ParticleSys.Emit(1);
        }
        else
        {
            var particles = new ParticleSystem.Particle[1];

            ParticleSys.GetParticles(particles);

            var particle = particles[0];
            particle.color = Color;
            particle.size = Size;
            particles[0]=particle;

            ParticleSys.SetParticles(particles, 1);
        }

    }



}
