/* ============================================================
   BIBLIOTHÈQUE — JavaScript 3D & Interactions
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

  // ===== EFFET 3D TILT SUR LES CARTES =====
  const cards = document.querySelectorAll('.item-card, .stat-card');

  cards.forEach(card => {
    card.addEventListener('mousemove', function (e) {
      const rect = card.getBoundingClientRect();
      const centerX = rect.left + rect.width / 2;
      const centerY = rect.top + rect.height / 2;
      const mouseX = e.clientX - centerX;
      const mouseY = e.clientY - centerY;

      const rotateX = -(mouseY / (rect.height / 2)) * 6;
      const rotateY = (mouseX / (rect.width / 2)) * 6;

      card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-8px) scale(1.01)`;
    });

    card.addEventListener('mouseleave', function () {
      card.style.transform = '';
    });
  });

  // ===== COMPTEUR ANIMÉ POUR LES STATS =====
  function animateCounter(el, target, duration = 1200) {
    const start = performance.now();
    const startVal = 0;

    function update(now) {
      const elapsed = now - start;
      const progress = Math.min(elapsed / duration, 1);
      const eased = 1 - Math.pow(1 - progress, 3); // ease-out cubic
      const current = Math.round(startVal + (target - startVal) * eased);
      el.textContent = current.toLocaleString('fr-FR');
      if (progress < 1) requestAnimationFrame(update);
    }

    requestAnimationFrame(update);
  }

  const statNumbers = document.querySelectorAll('.stat-number[data-target]');
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        const el = entry.target;
        const target = parseInt(el.dataset.target, 10);
        animateCounter(el, target);
        observer.unobserve(el);
      }
    });
  }, { threshold: 0.5 });

  statNumbers.forEach(el => observer.observe(el));

  // ===== APPARITION PROGRESSIVE DES CARTES =====
  const fadeEls = document.querySelectorAll('.item-card');
  const fadeObserver = new IntersectionObserver((entries) => {
    entries.forEach((entry, i) => {
      if (entry.isIntersecting) {
        setTimeout(() => {
          entry.target.style.opacity = '1';
          entry.target.style.transform = '';
        }, i * 60);
        fadeObserver.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });

  fadeEls.forEach(el => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(20px)';
    el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
    fadeObserver.observe(el);
  });

  // ===== AUTO-DISMISS ALERTS =====
  const alerts = document.querySelectorAll('.alert-biblio');
  alerts.forEach(alert => {
    setTimeout(() => {
      alert.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
      alert.style.opacity = '0';
      alert.style.transform = 'translateX(20px)';
      setTimeout(() => alert.remove(), 500);
    }, 4000);
  });

  // ===== CONFIRMATION SUPPRESSION =====
  document.querySelectorAll('form[data-confirm]').forEach(form => {
    form.addEventListener('submit', function (e) {
      const msg = form.dataset.confirm || 'Êtes-vous sûr de vouloir supprimer ?';
      if (!confirm(msg)) {
        e.preventDefault();
      }
    });
  });

  // ===== GLOW CURSOR SUR HERO =====
  const hero = document.querySelector('.hero-section');
  if (hero) {
    hero.addEventListener('mousemove', (e) => {
      const rect = hero.getBoundingClientRect();
      const x = ((e.clientX - rect.left) / rect.width) * 100;
      const y = ((e.clientY - rect.top) / rect.height) * 100;
      hero.style.setProperty('--glow-x', x + '%');
      hero.style.setProperty('--glow-y', y + '%');
    });
  }

  // ===== NAVBAR ACTIVE LINK =====
  const currentPath = window.location.pathname.toLowerCase();
  document.querySelectorAll('.nav-link-biblio').forEach(link => {
    const href = link.getAttribute('href')?.toLowerCase() || '';
    if (href !== '/' && currentPath.startsWith(href)) {
      link.classList.add('active');
    } else if (href === '/' && currentPath === '/') {
      link.classList.add('active');
    }
  });

  // ===== SEARCH: SUBMIT ON ENTER =====
  document.querySelectorAll('.search-input').forEach(input => {
    input.addEventListener('keydown', function (e) {
      if (e.key === 'Enter') {
        const form = input.closest('form');
        if (form) form.submit();
      }
    });
  });

});
