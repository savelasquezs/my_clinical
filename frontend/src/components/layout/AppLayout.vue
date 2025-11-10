<template>
  <div class="min-h-screen bg-gray-50">
    <Navbar />
    <div class="flex">
      <Sidebar />
      <main class="flex-1 p-6">
        <router-view />
      </main>
    </div>
    <Footer />
  </div>
</template>

<script setup>
import Navbar from './Navbar.vue'
import Sidebar from './Sidebar.vue'
import Footer from './Footer.vue'
</script>

