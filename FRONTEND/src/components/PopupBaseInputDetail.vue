<template>
  <b-modal dialog-class="modal-detail" scrollable :no-close-on-backdrop=true :title="label" :no-close-on-esc=true v-model="show" ok-title="Simpan" ok-variant="success" cancel-title="Batal" @hidden="$emit('close')">
    <div>
      <slot/>
    </div>
  </b-modal>
</template>

<script>
export default {
  name: 'PopupWrap',
  data() {
    return {
      show : true
    }
  },
  props: {
    centered: {
      default: true,
      type: Boolean,
    },
    focusElement: {
      default: null,
      type: Object,
    },
    label: {
      required: true,
      type: String,
    },
  },
  mounted() {
    const close = (e) => {
      const ESC = 27;
      if (e.keyCode !== ESC) return;
      this.$emit('close');
    };

    document.addEventListener('keyup', close);
    this.$on('hook:destroyed', () => {
      document.removeEventListener('keyup', close);
    });

    this.activate();
    this.$on('hook:destroyed', () => {
      this.deactivate();
    });
  },
  methods: {
    activate() {
      this.previousActiveElement = document.activeElement;
      this.inert();
    },
    async deactivate() {
      await this.inert(false);
    },
    async inert(status = true) {
      await this.$nextTick();
      [...this.$root.$el.children].forEach((child) => {
        if (child === this.$el || child.contains(this.$el)) return;
        child.inert = status;
      });
    },
  },
};
</script>

<style>
.modal-body {
  background: #EEF0F8 !important;
}
@media (min-width: 768px) {
  .modal-detail {
      width: 50% !important;
      height: 80% !important;
      margin-top: 4.75rem !important;
  } 
}
</style>