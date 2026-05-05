document.addEventListener('DOMContentLoaded', () => {
  const nav = document.querySelector('.glass-nav');
  const onScroll = () => nav && nav.classList.toggle('shadow-lg', window.scrollY > 20);
  onScroll(); window.addEventListener('scroll', onScroll);
});
