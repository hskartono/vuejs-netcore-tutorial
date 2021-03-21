<template>
  <Portal to="app-popup">
    <PopupBaseDetail
      :label="label"
      @copyDataMethod="copyDataAction"
      @close="$router.back()"
    >
      <slot name="backdrop" slot="backdrop"/>
      <slot/>
    </PopupBaseDetail>
  </Portal>
</template>

<script>
import PopupBaseDetail from './PopupBaseDetail.vue';

export default {
  name: 'PopupRouterViewDetail',
  props: {
    label: {
      required: true,
      type: String,
    },
    copyDataMethod: {}
  },
  components: {
    PopupBaseDetail,
  },
  methods : {
    copyDataAction(modalEvent, modl) {
      this.$emit("copyDataMethod", modalEvent);
    }
  }
};
</script>